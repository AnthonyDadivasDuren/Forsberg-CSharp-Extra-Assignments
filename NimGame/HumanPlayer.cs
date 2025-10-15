namespace NimGame;

public class HumanPlayer(string name = "Player") : IPlayer
{
    public string Name { get; } = name;
    public bool IsHuman => true;

    public int[] GetMove(int[] piles, int maxTake)
    {
        while (true)
        {
            Console.Write($"\n{Name}, choose a row (1–{piles.Length}): ");
            var rowInput = Console.ReadLine();

            if (!int.TryParse(rowInput, out var row) || row < 1 || row > piles.Length)
            {
                Console.WriteLine("Invalid row number.");
                continue;
            }

            if (piles[row - 1] == 0)
            {
                Console.WriteLine("That row is empty.");
                continue;
            }

            int available = piles[row - 1];
            int maxThisRow = Math.Min(maxTake, available);

            Console.Write($"How many matches do you want to draw (1–{maxThisRow}): ");
            var takeInput = Console.ReadLine();

            if (!int.TryParse(takeInput, out int take) || take < 1 || take > maxThisRow)
            {
                Console.WriteLine("Invalid number of matches.");
                continue;
            }

            return [row - 1, take];
        }
    }
}