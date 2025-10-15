namespace NimGame;

public interface IPlayer
{
    string Name { get; }
    bool IsHuman { get; }
  
    
    int[] GetMove(int[] piles, int maxTake);
}