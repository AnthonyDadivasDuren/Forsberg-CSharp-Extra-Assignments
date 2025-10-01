namespace NimGame;

public enum AiMode { Easy, Unbeatable }

public class AiPlayer(AiMode mode) : IPlayer
{
    private readonly Random _random = new Random();
    private AiMode Mode { get; } = mode;
    public string Name => Mode == AiMode.Unbeatable ? "AI (Unbeatable)" : "AI (Easy)";
    public bool IsHuman => false;

    public int GetMove(int currentMatches, int maxTake)
    {
        if (Mode == AiMode.Unbeatable)
        {
            
            if (currentMatches == 1)
                return 1; 

            int target = (currentMatches - 1) % 4;
            if (target == 0)
            {
                return Math.Min(1, maxTake); 
            }

            int move = Math.Min(target, maxTake);
            
            if (move >= currentMatches)
                move = Math.Max(1, Math.Min(maxTake, currentMatches - 1));

            return move;
        }

        
        return _random.Next(1, maxTake + 1);
    }
}