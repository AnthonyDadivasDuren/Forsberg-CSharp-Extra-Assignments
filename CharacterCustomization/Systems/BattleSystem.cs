using CharacterCustomization.Characters;


namespace CharacterCustomization.Systems
{
    public static class BattleSystem
    {
        public static void BattlePicker()
        {
            if (CharacterManager.Characters.Count < 2)
            {
                Console.WriteLine("Need at least 2 characters.");
                return;
            }

            Console.Clear();
            CharacterManager.DisplayCharacters();

            Console.Write("Choose your fighter #: ");
            int pi = int.Parse(Console.ReadLine() ?? "1") - 1;
            if (pi < 0 || pi >= CharacterManager.Characters.Count) { Console.WriteLine("Invalid."); return; }
            Character player = CharacterManager.Characters[pi];

            Console.Write("Choose opponent #: ");
            int ei = int.Parse(Console.ReadLine() ?? "1") - 1;
            if (ei < 0 || ei >= CharacterManager.Characters.Count || ei == pi) { Console.WriteLine("Invalid."); return; }
            Character enemy = CharacterManager.Characters[ei];

            RunBattle(player, enemy, awardXp: true, playerDmgMult: 1.0, enemyDmgMult: 1.0 , difficulty: DifficultyMods.Normal);
        }

        public static void RunBattle(
            Character player,
            Character enemy,
            bool awardXp,
            double playerDmgMult,
            double enemyDmgMult,
            DifficultyMods difficulty)          
        {
            var state = new BattleState(player, enemy, playerDmgMult, enemyDmgMult, difficulty);
            

            Console.Clear();
            Console.WriteLine($"Battle Start: {player.Name} vs {enemy.Name}");

            while (player.Health > 0 && enemy.Health > 0)
            {
                UiSystem.ShowHud(state, state.RoundNumber);

                Console.WriteLine();
                Console.Write("Choose action: 1 - 5: ");
                string act = Console.ReadLine() ?? "1";
                DoTurn(state, attacker: player, defender: enemy, act);
                if (enemy.Health <= 0) break;

                string enemyAct = AiController.DecideEnemyAction(state, enemy, player);
                Console.WriteLine();
                Console.WriteLine($"Enemy chooses: {UiSystem.ActionName(enemyAct)}");
                DoTurn(state, attacker: enemy, defender: player, enemyAct);
                if (player.Health <= 0) break;

                // End of round
                UiSystem.ClearBetweenSections();
                UiSystem.ShowDamageBreakdown(state);
                state.TickEndOfRound();
                UiSystem.Pause();
            }

            Console.WriteLine();
            Console.WriteLine("Battle Over");
            if (player.Health <= 0 && enemy.Health <= 0) Console.WriteLine("It is a draw.");
            else if (player.Health <= 0) Console.WriteLine($"{enemy.Name} wins.");
            else Console.WriteLine($"{player.Name} wins.");

            if (!awardXp) return;
            if (player.Health > 0) ExperienceSystem.AwardExperience(player, enemy);
            else ExperienceSystem.AwardExperience(enemy, player);
        }

        private static void DoTurn(BattleState s, Character attacker, Character defender, string choice)
        {
            if (s.IsStunned(attacker))
            {
                Console.WriteLine($"{attacker.Name} is stunned and loses the turn.");
                s.ClearStun(attacker);
                return;
            }

            if (attacker is ICharacterActions actions)
            {
                switch (choice)
                {
                    case "1": // Attack
                    {
                        int dmg = actions.Attack();
                        dmg = s.ApplyHitModifiers(attacker, defender, dmg);
                        dmg = s.ApplyDamageBuff(attacker, dmg);
                        ApplyDamage(s, defender, dmg);
                        break;
                    }
                    case "2": // Defend
                    {
                        actions.Defend();
                        s.SetDefend(attacker);
                        s.RoundLog.Add($"{attacker.Name} is defending");
                        break;
                    }
                    case "3": // Heal
                    {
                        int before = attacker.Health;
                        actions.Heal();
                        int healed = attacker.Health - before;
                        if (healed > 0) s.RoundLog.Add($"{attacker.Name} heals {healed} (HP {attacker.Health}/{attacker.MaxHealth})");
                        else s.RoundLog.Add($"{attacker.Name} fails to heal");
                        break;
                    }
                    case "4": // Recover
                    {
                        int before = UiSystem.GetEnergy(attacker);
                        actions.Recover();
                        int after = UiSystem.GetEnergy(attacker);
                        int delta = after - before;
                        if (delta > 0) s.RoundLog.Add($"{attacker.Name} recovers {delta} {UiSystem.EnergyName(attacker)}");
                        else s.RoundLog.Add($"{attacker.Name} fails to recover");
                        break;
                    }
                    case "5": // Special
                    {
                        s.Player.ClearSpecialOutcome();
                        s.Enemy.ClearSpecialOutcome();

                        int specialDmg = 0;

                        if (attacker is IAISpecial aiUser && defender != null && /* AI branch: you already know when it's AI */ attacker != s.Player)
                        {
                            // Enemy (AI) uses a special with defender context
                            specialDmg = aiUser.SpecialAttackAI(defender);
                        }
                        else if (attacker is ISpecial userSpecial)
                        {
                            // Show HUD without the 1–5 menu
                            UiSystem.ShowHud(s, s.RoundNumber, showMainMenu: false);

                            // Show special options
                            string specialChoice = UiSystem.ShowSpecialOptions(s, attacker);

                            if (specialChoice == "0")
                            {
                                // Player cancelled — return them to main action selection
                                Console.WriteLine("Special cancelled. Returning to main action menu...");
                                UiSystem.Pause();

                                // Rerun the turn input for this character
                                UiSystem.ShowHud(s, s.RoundNumber, showMainMenu: true);
                                Console.Write("Choose action [1-5]: ");
                                string newChoice = Console.ReadLine() ?? "1";
                                DoTurn(s, attacker, defender, newChoice); // restart this turn with a new choice
                                return; // stop here so we don't continue the original turn
                            }

                            // If not cancelled → proceed with the chosen special
                            specialDmg = userSpecial.SpecialAttack(specialChoice);
                        }
                        else
                        {
                            Console.WriteLine($"{attacker.Name} has no special move.");
                            s.RoundLog.Add($"{attacker.Name} has no special");
                            break;
                        }

                        specialDmg = s.ApplyHitModifiers(attacker, defender, specialDmg);
                        specialDmg = s.ApplyDamageBuff(attacker, specialDmg); 
                        
                        int cap = (int)(defender.MaxHealth * s.SpecialCapPercent());
                        if (specialDmg > cap) specialDmg = cap;
                        
                        ApplyDamage(s, defender, specialDmg);

                        // Apply the effect requested by the special (if any)
                        var outcome = attacker.LastSpecialOutcome;
                        if (outcome.HasValue)
                        {
                            var o = outcome.Value;
                            if (o.SelfTarget)
                            {
                                if (o.Effect == "shield") s.ApplyHolyShield(attacker, o.Potency);
                                if (o.Effect == "defend") s.SetDefend(attacker);
                            }
                            else
                            {
                                if (o.Effect == "stun")  s.ApplyStun(defender, o.Turns);
                                if (o.Effect == "burn")  s.ApplyBurn(defender, o.Turns, o.Potency);
                                if (o.Effect == "bleed") s.ApplyBleed(defender, o.Turns, o.Potency);
                            }
                            attacker.ClearSpecialOutcome();
                        }

                        break;
                    }
                    default:
                        Console.WriteLine($"{attacker.Name} hesitates and loses the turn.");
                        s.RoundLog.Add($"{attacker.Name} lost the turn");
                        break;
                }
            }
        }

        private static void ApplyDamage(BattleState s, Character target, int dmg)
        {
            if (dmg <= 0)
            {
                s.RoundLog.Add($"{target.Name} takes no damage");
                return;
            }
            int before = target.Health;
            target.Health = Math.Max(0, target.Health - dmg);
            int taken = before - target.Health;
            Console.WriteLine($"{target.Name} takes {taken} damage. (HP {target.Health}/{target.MaxHealth})");
            s.RoundLog.Add($"{target.Name} took {taken} damage");
        }
    }
}
