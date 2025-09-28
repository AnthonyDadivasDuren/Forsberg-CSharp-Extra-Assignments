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

//todo
// add failsafes so input can only be numbers
//limit player choice
//add menu 
// code cleanup

int currentMatches = 24;
int matchesDrawn = 0;
char match = '|';
string output = "";

int choiceRange = 3;
int aichoice = 0;
int playerChoiche = 0;

int currentTurn = 1;

bool isRunning = true;
bool win = false;
bool gameOver = false;
 
Random random = new Random();

Console.WriteLine("Welcome to Nim");


 while (true)
 {
     
     output = "";
     
     if (currentMatches < 1)
         break;
     

     
     for (int i = 0; i < currentMatches; i++)
     {
         output += match;
     }
     Console.Clear();
     Console.WriteLine($"{output} ({currentMatches})");
     
     
     choiceRange = Math.Min(3, currentMatches);

     if (currentTurn % 2 != 0)
     {
         PlayerTurn();
     }
     else
     {
         AITurn();
     }
 }
 

 void PlayerTurn()
 {
     Console.WriteLine("How many matches do you want to draw?");
     playerChoiche = int.Parse(Console.ReadLine());

     if (playerChoiche < 1 || playerChoiche > Math.Min(3, currentMatches))
     {
         Console.WriteLine("Invalid choice. Try again.");
         return;
     }

     currentMatches -= playerChoiche;

     if (currentMatches <= 0)
     {
         gameOver = true;
         Console.WriteLine("You drew the last match. You lose!");
         Console.ReadLine();
     }
     else
     {
         currentTurn++;
     }
 }

 void AITurn()
 {
     aichoice = random.Next(1, Math.Min(3, currentMatches) + 1);
     Console.WriteLine($"AI draws {aichoice} matches.");
     Console.ReadLine();

     currentMatches -= aichoice;

     if (currentMatches <= 0)
     {
         win = true;
         Console.WriteLine("AI drew the last match. You win!");
         Console.ReadLine();
     }
     else
     {
         currentTurn++;
     }
 } 

 
 