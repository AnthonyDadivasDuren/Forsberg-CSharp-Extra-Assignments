using CharacterCustomization.Systems;

bool running = true;

while (running)
{
    Console.Clear();
    Console.WriteLine("--- Main Menu ---");
    Console.WriteLine("1. Create character");
    Console.WriteLine("2. Display characters");
    Console.WriteLine("3. Battle (pick two from list)");
    Console.WriteLine("4. Arcade Mode");
    Console.WriteLine("5. Exit");
    Console.Write("Choose: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            CharacterManager.CreateCharacter();
            UiSystem.Pause();
            break;
        case "2":
            CharacterManager.DisplayCharacters();
            UiSystem.Pause();
            break;
        case "3":
            BattleSystem.BattlePicker();
            UiSystem.Pause();
            break;
        case "4":
            ArcadeMode.StartArcade();
            UiSystem.Pause();
            break;
        case "5":
            running = false;
            break;
        default:
            Console.WriteLine("Invalid option.");
            UiSystem.Pause();
            break;
    }
}