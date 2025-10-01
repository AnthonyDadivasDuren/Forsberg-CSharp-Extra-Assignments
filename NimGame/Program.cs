/*
 * Goal
Recap everything that you have learned this week through an advanced exercise. Pick either the Nim-Game, the Tic-Tac-Toe-Game or the Battleship-Game depending on your confidence in your abilities.
You are allowed to complete all three games, if you feel that you are up for the challenge.

------------------
Output:Welcome to Nim!
Output:|||||||||||||||||||||||| (24)
Output:How many matches do you want to draw?
Input:3
Output:||||||||||||||||||||| (21)
Output:The AI draws 3 matches.
Output:|||||||||||||||||| (18)
...
Output:|||
Output:The AI draws 2 matches.
Output:|
Output:How many matches do you want to draw?
Input: 1
Output:You drew the last match. You lose.
-------------------

2 players
24 matches
Players take turns to draw matches
Each player may draw 1,2 or 3 matches (not more or less)
The Player who has to take last match loses
Make an ASCII-Art Display of the remaining matches ||||||||
Implement an AI-Player

    Can you make it fun?
    Can you make it unbeatable?
 */

namespace NimGame
{
    static class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== NIM ====");
                Console.WriteLine("1) Player vs Player ");
                Console.WriteLine("2) Play vs AI (Easy)");
                Console.WriteLine("3) Play vs AI (Unbeatable)");
                Console.WriteLine("3) Quit");
                Console.Write("Choice: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    IPlayer human1 = new HumanPlayer("Player 1");
                    IPlayer human2 = new HumanPlayer("Player 2");
                    
                    var game = new NimGame(human1, human2, startingMatches: 24);
                    game.Run();
                }
                
                if (choice == "2" || choice == "3")
                {
                    IPlayer human = new HumanPlayer("You");
                    AiMode mode = choice == "1" ? AiMode.Easy : AiMode.Unbeatable;
                    IPlayer ai = new AiPlayer(mode);

                    var game = new NimGame(human, ai, startingMatches: 24);
                    game.Run();

                    Console.Write("\nPlay again? (y/n): ");
                    var again = Console.ReadLine() ?? "";
                    again = again.ToLower();
                    if ( again == "y")
                        break;
                }
                else if (choice == "4")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }
    }
}