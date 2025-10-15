using CharacterCustomization.Characters;


namespace CharacterCustomization.Systems

{
    public static class ShopSystem
    {
        private static int _potionPrice = 20;
        private static int _hpPrice = 50;
        private static int _dmgPrice = 70;

        public static void Open(Character player, ref int gold, ref double playerDmgBuff)
        {
            bool shopping = true;
            while (shopping)
            {
                Console.Clear();
                Console.WriteLine("Shop");
                Console.WriteLine($"Gold: {gold}");
                Console.WriteLine($"Potions: {UiSystem.GetPotions(player)}");
                Console.WriteLine($"Damage Buff (this run): x{playerDmgBuff:0.00}");
                Console.WriteLine();
                Console.WriteLine($"1. Buy healing potion ({_potionPrice} gold)");
                Console.WriteLine($"2. Train body (+10 Max HP) ({_hpPrice} gold)");
                Console.WriteLine($"3. Sharpen weapon (+5% damage this run) ({_dmgPrice} gold)");
                Console.WriteLine("4. Leave shop");
                Console.Write("Choose: ");
                string choice = Console.ReadLine() ?? "4";

                switch (choice)
                {
                    case "1":
                        BuyPotion(player, ref gold);
                        break;

                    case "2":
                        TrainBody(player, ref gold);
                        break;

                    case "3":
                        UpgradeWeapon(ref gold, ref playerDmgBuff);
                        break;

                    case "4":
                        shopping = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                UiSystem.Pause();
            }
        }

        private static void BuyPotion(Character player, ref int gold)
        {
            if (gold < _potionPrice)
            {
                Console.WriteLine("Not enough gold.");
                return;
            }

            gold -= _potionPrice;
            if      (player is Mage m)    m.HealingPotions++;
            else if (player is Warrior w) w.HealingPotions++;
            else if (player is Rogue r)   r.HealingPotions++;
            else if (player is Paladin p) p.HealingPotions++;

            Console.WriteLine("Bought 1 potion.");
            _potionPrice = (int)(_potionPrice * 1.1);
        }

        private static void TrainBody(Character player, ref int gold)
        {
            if (gold < _hpPrice)
            {
                Console.WriteLine("Not enough gold.");
                return;
            }

            gold -= _hpPrice;
            player.MaxHealth += 10;
            player.Health += 10;

            Console.WriteLine("Max health increased by 10.");
            _hpPrice = (int)(_hpPrice * 1.1);
        }

        private static void UpgradeWeapon(ref int gold, ref double playerDmgBuff)
        {
            if (gold < _dmgPrice)
            {
                Console.WriteLine("Not enough gold.");
                return;
            }

            gold -= _dmgPrice;
            playerDmgBuff *= 1.05;

            Console.WriteLine($"Damage buff is now x{playerDmgBuff:0.00} for this arcade run.");
            _dmgPrice = (int)(_dmgPrice * 1.15);
        }
    }
}