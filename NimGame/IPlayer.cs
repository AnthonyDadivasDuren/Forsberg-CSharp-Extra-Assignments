namespace NimGame;

public interface IPlayer
{
    string Name { get; }
    bool IsHuman { get; }
  
    
    int GetMove(int currentMatches, int maxTake);
}