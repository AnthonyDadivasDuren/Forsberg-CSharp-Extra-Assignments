namespace NimGame;


public class HumanPlayer : IPlayer
{
    public string Name { get; }
    public bool IsHuman => true;

    public HumanPlayer(string name = "Player")
    {
        Name = name;
    }

    public int GetMove(int currentMatches, int maxTake)
    {
        while (true)
        {
            Console.Write($"\nHow many matches do you want to draw? (1–{maxTake}): ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input! Please enter a number.");
                continue;
            }

            if (choice < 1 || choice > maxTake)
            {
                Console.WriteLine($"You can only take between 1 and {maxTake}.");
                continue;
            }

            return choice;
        }
    }
}