using CSharpPlayersGuide.RichConsole;
using System.Numerics;
using System.Windows.Input;


//THIS IS THE ENTRY POINT
RichConsole.WriteLine("Welcome to Matt's Fountain of Objects!", TextEffects.DoubleUnderline);
RichConsole.WriteLine("Make sure to fullscreen! Double click the top bar! You may need to adjust zoom level with LCTRL + Mousewheel", Colors.Aqua, TextEffects.Blink);
Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
Console.SetWindowPosition(0, 0);
Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.Black;


Game game = new Game();
// A new game has all data ready to go. 
game.StartGame();



//Game
class Game
{

    public Difficulty difficulty;
    public Grid GameGrid;
    public Player player { get; set; }

    public Game()
    {
        // GRID CONSTRUCTOR
        GameGrid = new Grid(SetupDifficulty());
        player = new Player();
    }

    public void StartGame()
    {
        while (true)
        {
            RichConsole.Clear();
            GameGrid.DrawGameView();
            RichConsole.WriteLine("GAME VIEW");
            RichConsole.WriteLine("CONTROLS - ARROWS KEYS TO MOVE - M FOR MAP - SPACE TO PREPARE BOW - ESC FOR QUIT");
            // Draw sensations here. 

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                    RichConsole.Write("BOW ARMED CHOOSE A DIRECTION - SPACEBAR TO DISENAGE BOW - ARROWS TO FIRE");
                    var key2 = Console.ReadKey(true);
                    switch (key2.Key)
                    {
                        case ConsoleKey.Spacebar:
                            break;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    TryMovePlayer(-1, 0); // Move x by -1, y by 0
                    break;
                case ConsoleKey.DownArrow:
                    TryMovePlayer(1, 0); // Move x by +1, y by 0
                    break;
                case ConsoleKey.LeftArrow:
                    TryMovePlayer(0, -1); // Move x by 0, y by -1
                    break;
                case ConsoleKey.RightArrow:
                    TryMovePlayer(0, 1); // Move x by 0, y by +1
                    break;
                case ConsoleKey.Escape:
                    return;
                case ConsoleKey.M:
                    Console.Beep(100,1);
                    RichConsole.Clear();
                    GameGrid.DrawFullMap();
                    RichConsole.WriteLine("MAP VIEW -- Press M to Close");
                    while (Console.ReadKey().Key != ConsoleKey.M)
                    {
                        //Wait for map to be closed
                    }
                    Console.Beep(100, 1);
                    break;
            }


        }
    }

    private void TryMovePlayer(int tryX, int tryY)
    {
        int targetX = player.x + tryX;
        int targetY = player.y + tryY;

        // Check for Room Boundaries (Entering/Exiting room and Exiting Cave when game complete)
        if (targetX < 0 || targetX > 4 || targetY < 0 || targetY > 4)
        {
            //HandleRoomTransition();
            Console.Beep(1000, 10);
            return;
        }

        // Check for Solid Walls
        if (GameGrid.PlayerRoom.TilesData[targetX, targetY].TileType == TileType.Solid)
        {
            Console.Beep(7000, 1); // Wall hit sound
            return;
        }

        // Update Position
        player.x = targetX;
        player.y = targetY;
        GameGrid.Entrance.PlayerPosition = new Vector2(targetX, targetY);
        Console.Beep(4000, 1);
    }

    private int SetupDifficulty()
    {
        do
        {
            RichConsole.Write("Input your desired difficulty: ");
            string? input = Console.ReadLine();

            switch (input.ToLower())
            {
                case "test":
                    difficulty = Difficulty.Unset;
                    return 3;
                case "easy":
                    difficulty = Difficulty.Easy;
                    return 4;
                case "medium":
                    difficulty = Difficulty.Medium;
                    return 6;
                case "hard":
                    difficulty = Difficulty.Hard;
                    return 8;
                case "insane":
                    difficulty = Difficulty.Insane;
                    return 10;
                case "hurtme":
                    difficulty = Difficulty.HurtMe;
                    return 20;
            }
            RichConsole.WriteLine("Error: Not a valid input!", Colors.Red, TextEffects.Blink);
        }
        while (difficulty == Difficulty.Unset);
        return 0;
    }
}