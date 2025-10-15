namespace NimGame;

public enum AiMode { Easy, Unbeatable }

public class AiPlayer(AiMode mode) : IPlayer
{
    private readonly Random _random = new Random();
    private AiMode Mode { get; } = mode;
    public string Name => Mode == AiMode.Unbeatable ? "AI (Unbeatable)" : "AI (Easy)";
    public bool IsHuman => false;

    public int[] GetMove(int[] piles, int maxTake)
    {
        if (Mode == AiMode.Unbeatable)
        {
            return GetOptimalMove(piles, maxTake);
        }

        int row;
        do
        {
            row = _random.Next(0, piles.Length);
        } while (piles[row] == 0);

        int take = _random.Next(1, Math.Min(maxTake, piles[row]) + 1);
        return [row, take];
    }

    private int[] GetOptimalMove(int[] piles, int maxTake)
    {
        int nimSum = 0;
        foreach (int pile in piles)
        {
            nimSum ^= pile;
        }

       
        if (nimSum == 0)
        {
            int row;
            do
            {
                row = _random.Next(0, piles.Length);
            } while (piles[row] == 0);

            int take = _random.Next(1, Math.Min(maxTake, piles[row]) + 1);
            return [row, take];
        }

       
        for (int i = 0; i < piles.Length; i++)
        {
            int desired = piles[i] ^ nimSum; 
            if (desired < piles[i])
            {
                int take = piles[i] - desired;

             
                take = Math.Min(take, Math.Min(maxTake, piles[i]));
                if (take > 0)
                {
                    return [i, take];
                }
            }
        }

     
        int fallbackRow;
        do
        {
            fallbackRow = _random.Next(0, piles.Length);
        } while (piles[fallbackRow] == 0);

        int fallbackTake = _random.Next(1, Math.Min(maxTake, piles[fallbackRow]) + 1);
        return [fallbackRow, fallbackTake];
    }
}