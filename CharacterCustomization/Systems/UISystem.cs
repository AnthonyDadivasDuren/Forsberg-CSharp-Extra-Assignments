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
                Console.WriteLine("[1] Attack   [2] Defend   [3] Heal   [4] Recover   [5] Special");
                Console.WriteLine("-----------------------------------------------");
            }
        }
        
        public static string ShowSpecialOptions(BattleState s, Character c)
        {
            
            // todo --- display correct attacks and also the damage range the attack could give
            Console.WriteLine();
            Console.WriteLine("----------- Special Attacks -----------");

            if (c is Warrior)
            {
                Console.WriteLine("[1] Whirlwind Strike  - Medium AoE");
                Console.WriteLine("[2] Earthshatter Slam - Heavy damage, stun chance");
                Console.WriteLine("[3] Second Wind       - Heal & stamina regen");
                Console.WriteLine("[4] Rocket Strike  - Medium AoE");
                
            }
            else if (c is Mage)
            {
                Console.WriteLine("[1] Firestorm         - Burn over time");
                Console.WriteLine("[2] Arcane Surge      - Burst damage");
                Console.WriteLine("[3] Mana Infusion     - Regen & shield");
            }
            else if (c is Rogue)
            {
                Console.WriteLine("[1] Shadow Cut      - low dmg + bleed (2T)");
                Console.WriteLine("[2] Ambush          - solid dmg, high crit");
                Console.WriteLine("[3] Death Mark      - heavy damage");
                Console.WriteLine("[4] Smoke Bomb      - small dmg + defend self");
            }
            else if (c is Paladin)
            {
                Console.WriteLine("[1] Holy Smite        - High faith burst");
                Console.WriteLine("[2] Divine Shield     - Strong shield & cleanse");
                Console.WriteLine("[3] Judgement         - Damage + burn");
            }

            Console.WriteLine("---------------------------------------");
            Console.WriteLine("[0] Cancel and return");
            Console.Write("Choose special [1-3]: ");
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
