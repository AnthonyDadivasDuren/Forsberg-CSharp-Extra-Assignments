using System;
using System.Collections.Generic;
using CharacterCustomization.Characters;


namespace CharacterCustomization.Systems
{
    public static class CharacterManager
    {
        public static List<Character> Characters { get; } = new();

        public static void CreateCharacter()
        {
            Console.Clear();
            Console.WriteLine("Choose class:");
            Console.WriteLine("1. Warrior");
            Console.WriteLine("2. Mage");
            Console.WriteLine("3. Rogue");
            Console.WriteLine("4. Paladin");
            Console.Write("Your choice: ");
            string t = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter name: ");
            string name = Console.ReadLine() ?? "Hero";

            Console.Write("Enter level (1-50): ");
            int level = Math.Clamp(int.TryParse(Console.ReadLine(), out var lv) ? lv : 1, 1, 50);

            Character? c = t switch
            {
                "1" => new Warrior(name, level),
                "2" => new Mage(name, level),
                "3" => new Rogue(name, level),
                "4" => new Paladin(name, level),
                _ => null
            };

            if (c == null)
            {
                Console.WriteLine("Invalid class.");
                return;
            }

            Characters.Add(c);
            Console.WriteLine($"Created: {c}");
        }

        public static void DisplayCharacters()
        {
            Console.Clear();
            Console.WriteLine("--- Characters ---");
            if (Characters.Count == 0) { Console.WriteLine("None yet."); return; }
            for (int i = 0; i < Characters.Count; i++)
                Console.WriteLine($"{i + 1}. {Characters[i]}");
        }
    }
}