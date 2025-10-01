namespace NimGame;

using System;

public class NimGame(IPlayer player1, IPlayer player2, int startingMatches = 24, char matchChar = '|')
{
    private int _currentMatches = startingMatches;

    public void Run()
    {
        int currentTurn = 0; 

        while (true)
        {
            DisplayBoard();

            IPlayer current = currentTurn == 0 ? player1 : player2;
            IPlayer opponent = currentTurn == 0 ? player2 : player1;

            int maxTake = Math.Min(3, _currentMatches);
            int take = current.GetMove(_currentMatches, maxTake);

            
            if (take < 1 || take > maxTake)
            {
                Console.WriteLine($"Invalid move by {current.Name}. Turn skipped.");
                Console.ReadLine();
                continue;
            }

            if (!current.IsHuman)
            {
                Console.WriteLine($"\n{current.Name} draws {take} match(es).");
                Console.ReadLine();
            }

            _currentMatches -= take;

            if (_currentMatches <= 0)
            {
                Console.WriteLine($"\n{current.Name} drew the last match. {opponent.Name} wins!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }
            
            currentTurn = 1 - currentTurn;
        }
    }

    private void DisplayBoard()
    {
        Console.Clear();
        Console.WriteLine("==== NIM ====");
        Console.WriteLine(RenderMatches(_currentMatches));
        Console.WriteLine($"({_currentMatches})");
    }

    private string RenderMatches(int count)
    {
        string result = "";

        for (int i = 1; i <= count; i++)
        {
            result += matchChar; 

            if (i % 4 == 0 && i != count)
                result += " "; 
        }

        return result;
    }
}