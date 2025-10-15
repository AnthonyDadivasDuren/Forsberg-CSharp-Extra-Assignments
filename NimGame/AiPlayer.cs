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
            return GetOptimalMove(piles, maxTake);

        
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
        
        for (int i = 0; i < piles.Length; i++)
        {
            if (piles[i] == 0) continue;

            for (int take = 1; take <= Math.Min(maxTake, piles[i]); take++)
            {
                int[] newState = (int[])piles.Clone();
                newState[i] -= take;

                if (IsWinningState(newState))
                    return [i, take];
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
    
    private bool IsWinningState(int[] piles)
    {
        int nimSum = 0;
        int total = 0;
        int pilesGreaterThanOne = 0;

        foreach (int pile in piles)
        {
            nimSum ^= pile;
            total += pile;
            if (pile > 1) pilesGreaterThanOne++;
        }

        bool isEndGamePhase = (pilesGreaterThanOne == 0) || (total <= piles.Length + 1);

        if (isEndGamePhase)
        {
            int ones = 0;
            foreach (int pile in piles)
                if (pile == 1) ones++;

            return ones % 2 == 1;
        }
        else
        {
            
            return nimSum == 0;
        }
    }
}