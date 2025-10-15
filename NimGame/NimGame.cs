namespace NimGame;

using System;
using System.Linq;

public class NimGame(IPlayer player1, IPlayer player2, int[]? piles = null, int maxTake = 3, char matchChar = '|')
{
    private readonly int[] _piles = piles ?? [3, 4, 5];

    public void Run()
    {
        int currentTurn = 0;

        while (true)
        {
            DisplayBoard();

            IPlayer current = currentTurn == 0 ? player1 : player2;
            IPlayer opponent = currentTurn == 0 ? player2 : player1;

            int[] move = current.GetMove(_piles, maxTake);
            int row = move[0];
            int take = move[1];
            
           
            Console.WriteLine($"\n{current.Name} took {take} matches from row {row + 1}.");
            Console.ReadLine(); 
            

            _piles[row] -= take;

            if (IsGameOver())
            {
                Console.Clear();
                Console.WriteLine($"\n{current.Name} took the last match. {opponent.Name} win!");
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

        for (int i = 0; i < _piles.Length; i++)
        {
            Console.Write($"Row {i + 1}: ");
            Console.WriteLine(new string(matchChar, _piles[i]));
        }
    }

    private bool IsGameOver()
    {
        return _piles.All(p => p == 0);
    }
}