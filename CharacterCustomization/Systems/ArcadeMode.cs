using CharacterCustomization.Characters;

namespace CharacterCustomization.Systems
{
    public static class ArcadeMode
    {
        public static void StartArcade()
        {
            Console.Clear();
            Console.WriteLine("Arcade Mode");
            
            Console.WriteLine("Difficulty: 1. Easy  2. Normal  3. Hard");
            Console.Write("Choose difficulty: ");
            var d = Console.ReadLine();
            DifficultyMods diff = d switch
            {
                "1" => DifficultyMods.Easy,
                "3" => DifficultyMods.Hard,
                _   => DifficultyMods.Normal
            };
            
            Console.Clear();
            Console.WriteLine($"Difficulty set to: {diff.Name}");

            Console.ReadLine();
            
            Console.Clear();
            Console.WriteLine("1. Warrior  2. Mage  3. Rogue  4. Paladin");
            Console.Write("Choose class: ");
            string t = Console.ReadLine() ?? "1";

            Console.Write("Enter fighter name: ");
            string name = Console.ReadLine() ?? "Hero";
            
            Console.Clear();
            
            Console.Write("Enter starting level (1-50): ");
            int startLevel = Math.Clamp(int.TryParse(Console.ReadLine(), out var lv) ? lv : 1, 1, 50);
            
            Console.Clear();
            
            Console.Write("How many rounds? (e.g., 5, 10, 15): ");
            int rounds = Math.Clamp(int.TryParse(Console.ReadLine(), out var rn) ? rn : 10, 1, 50);

            
            Character player = t switch
            {
                "1" => new Warrior(name, startLevel),
                "2" => new Mage(name, startLevel),
                "3" => new Rogue(name, startLevel),
                "4" => new Paladin(name, startLevel),
                _ => new Warrior(name, startLevel)
            };

            int gold = 100;
            double playerDmgBuff = 1.0;

            for (int round = 1; round <= rounds; round++)
            {
                Console.Clear();
                Console.WriteLine($"--- Round {round}/{rounds} ---");

            
                var enemy = GenerateOpponent(round, player.Level);
                double enemyDmgMult = 1.0 + (round * 0.08); // +8% per round

                
                enemy.MaxHealth = (int)(enemy.MaxHealth * (1.0 + round * 0.08));
                enemy.Health = enemy.MaxHealth;

                Console.WriteLine($"Opponent: {enemy}  (Enemy DMG x{enemyDmgMult:0.00})");

                BattleSystem.RunBattle(player, enemy,
                    awardXp: true,
                    playerDmgMult: playerDmgBuff,
                    enemyDmgMult: enemyDmgMult * diff.EnemyDamageMult,
                    difficulty: diff);
                
                if (player.Health <= 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("You were defeated. Game Over.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Battle complete. Press Enter to continue to rewards.");
                Console.ReadLine();

                int reward = 30 + (enemy.Level * 5);
                gold += reward;
                Console.WriteLine($"Gold earned: {reward}. Current gold: {gold}");

                ShopSystem.Open(player, ref gold, ref playerDmgBuff);

                // Small recovery between rounds
                player.Health = Math.Min(player.MaxHealth, player.Health + 25);
                if      (player is Mage pm)     pm.Mana     += 40;
                else if (player is Warrior pw)  pw.Stamina  += 30;
                else if (player is Rogue pr)    pr.Energy   += 35;
                else if (player is Paladin pp)  pp.Zeal     += 28;

                Console.WriteLine("Recovered a bit before next round.");
                UiSystem.Pause();
            }

            Console.WriteLine("You completed all rounds. Arcade Champion.");
        }

        private static Character GenerateOpponent(int round, int playerCurrentLevel)
        {
            // level grows with round and with player's current level
            int level = Math.Min(50, (int)(playerCurrentLevel * 0.8 + round * 2));
            string name = $"CPU-{round}";
            int roll = SharedRng.Next(1, 5);
            return roll switch
            {
                1 => new Warrior(name, level),
                2 => new Mage(name, level),
                3 => new Rogue(name, level),
                _ => new Paladin(name, level)
            };
        }
    }
}
