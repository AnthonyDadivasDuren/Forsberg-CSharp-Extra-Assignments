using CharacterCustomization.Characters;

namespace CharacterCustomization.Systems
{
    public static class UiSystem
    {
        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }

        public static void ClearBetweenSections()
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 60));
            Console.WriteLine();
        }

        public static void ShowHud(BattleState s, int round, bool showMainMenu = true)
        {
            var p = s.Player;
            var e = s.Enemy;

            string pClass = p.GetType().Name;
            string eClass = e.GetType().Name;

            Console.Clear();
            Console.WriteLine($"================ ROUND {round} ================");
            
            Console.WriteLine($"{p.Level} {p.Name} [{pClass}]  HP {p.Health}/{p.MaxHealth}   {EnergyName(p)}: {GetEnergy(p)}   Potions: {GetPotions(p)}   {s.StatusText(p)}"); 
            Console.WriteLine($"{e.Level} {e.Name} [{eClass}]  HP {e.Health}/{e.MaxHealth}   {EnergyName(e)}: {GetEnergy(e)}   Potions: {GetPotions(e)}   {s.StatusText(e)}");
            Console.WriteLine("-----------------------------------------------");

            if (showMainMenu)
            {
                int potionHeal = 15 + p.Level * 5;
                int attackDmgLow = p.Level * 3 + 15;
                int attackDmgHigh = (int)(p.Level * 4.5 + 30);
                int recoverAmount = p switch
                {
                    Warrior => (int)(5 + p.Level * 0.6),
                    Mage    => (int)(5 + p.Level * 0.7),
                    Rogue   => (int)(5 + p.Level * 0.4),
                    Paladin => (int)(5 + p.Level * 0.5),
                    _       => 5
                };

                Console.WriteLine("[1] Attack   - ~{0}-{1} dmg, restores resource", attackDmgLow, attackDmgHigh);
                Console.WriteLine("[2] Defend   - Reduce incoming damage by ~50%");
                Console.WriteLine("[3] Heal     - Use potion (~{0} HP)", potionHeal);
                Console.WriteLine("[4] Recover  - Gain ~{0} resource", recoverAmount);
                Console.WriteLine("[5] Special  - Open special attack menu");
                Console.WriteLine("-----------------------------------------------");
            }
        }
        
       public static string ShowSpecialOptions(BattleState s, Character c)
    { 
        Console.WriteLine();
        Console.WriteLine("----------- Special Attacks -----------");

        if (c is Warrior w)
        {
            Console.WriteLine($"[1] Shield Bash     - Cost: 12 Stamina - ~{w.Level * 5 + 20} dmg - Stun 1T");
            Console.WriteLine($"[2] Power Strike    - Cost: 18 Stamina - ~{w.Level * 7 + 30} dmg - High crit");
            Console.WriteLine($"[3] Whirlwind       - Cost: 28 Stamina - ~{w.Level * 8 + 40} dmg - AoE (capped)");
            Console.WriteLine($"[4] Rallying Guard  - Cost: 15 Stamina - ~{w.Level * 4 + 15} dmg - Gain shield");
        }
        else if (c is Mage m)
        {
            Console.WriteLine($"[1] Frostbolt       - Cost: 12 Mana    - ~{m.Level * 5 + 25} dmg - Chill (DoT)");
            Console.WriteLine($"[2] Fire Blast      - Cost: 20 Mana    - ~{m.Level * 7 + 40} dmg - Burn (2T)");
            Console.WriteLine($"[3] Arcane Surge    - Cost: 30 Mana    - ~{m.Level * 8 + 55} dmg - Burst (capped)");
            Console.WriteLine($"[4] Mana Shield     - Cost: 18 Mana    - ~{m.Level * 4 + 20} dmg - Gain shield");
        }
        else if (c is Rogue r)
        {
            Console.WriteLine($"[1] Shadow Cut      - Cost: 12 Energy  - ~{r.Level * 5 + 20} dmg - Bleed 2T");
            Console.WriteLine($"[2] Ambush          - Cost: 20 Energy  - ~{r.Level * 6 + 30} dmg - High crit");
            Console.WriteLine($"[3] Death Mark      - Cost: 30 Energy  - ~{r.Level * 7 + 40} dmg - Heavy hit (capped)");
            Console.WriteLine($"[4] Smoke Bomb      - Cost: 15 Energy  - ~{r.Level * 3 + 15} dmg - Defend self");
        }
        else if (c is Paladin p)
        {
            Console.WriteLine($"[1] Holy Smite      - Cost: 14 Zeal    - ~{p.Level * 5 + 20} dmg - Burn 1T");
            Console.WriteLine($"[2] Divine Shield   - Cost: 18 Zeal    - ~{p.Level * 4 + 18} dmg - Gain shield");
            Console.WriteLine($"[3] Judgement       - Cost: 24 Zeal    - ~{p.Level * 7 + 38} dmg - Stun");
            Console.WriteLine($"[4] Consecration    - Cost: 28 Zeal    - ~{p.Level * 6 + 45} dmg - Burn 2T");
        }

        Console.WriteLine("---------------------------------------");
        Console.WriteLine("[0] Cancel and return");
        Console.Write("Choose special [1-4]: ");
        return Console.ReadLine() ?? "0";
    }

        public static void ShowDamageBreakdown(BattleState s)
        {
            if (s.RoundLog.Count == 0) return;
            Console.WriteLine();
            Console.WriteLine("Turn summary:");
            foreach (var line in s.RoundLog)
                Console.WriteLine(" - " + line);
            s.RoundLog.Clear();
        }

        public static string EnergyName(Character c)
            => c is Mage ? "Mana"
             : c is Warrior ? "Stamina"
             : c is Rogue ? "Energy"
             : c is Paladin ? "Zeal"
             : "Energy";

        public static int GetEnergy(Character c) => c switch
        {
            Mage m => m.Mana,
            Warrior w => w.Stamina,
            Rogue r => r.Energy,
            Paladin p => p.Zeal,
            _ => 0
        };

        public static int GetPotions(Character c) => c switch
        {
            Mage m => m.HealingPotions,
            Warrior w => w.HealingPotions,
            Rogue r => r.HealingPotions,
            Paladin p => p.HealingPotions,
            _ => 0
        };

        public static string ActionName(string a) => a switch
        {
            "1" => "Attack",
            "2" => "Defend",
            "3" => "Heal",
            "4" => "Recover",
            "5" => "Special",
            _ => "Wait"
        };
    }
}
