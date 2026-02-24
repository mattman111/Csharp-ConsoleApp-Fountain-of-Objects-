using CSharpPlayersGuide.RichConsole;
using System.Numerics;



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

//TODO: 
// (2) Draw 8 rooms around the players starting position [GAME VIEW]. (HOW DO I DO THIS?)



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
            GameGrid.DrawFullMap();
            RichConsole.WriteLine("GAME VIEW");
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    Math.Clamp(player.y += 1, 0, 4);
                    GameGrid.Entrance.PlayerPosition = new Vector2(player.x, player.y);
                    break;
                case ConsoleKey.DownArrow:
                    Math.Clamp(player.y -= 1, 0, 4);
                    GameGrid.Entrance.PlayerPosition = new Vector2(player.x, player.y);
                    break;
                case ConsoleKey.LeftArrow:
                    break;
                case ConsoleKey.RightArrow:
                    break;
                case ConsoleKey.M:
                    RichConsole.Clear();
                    GameGrid.DrawFullMap();
                    RichConsole.WriteLine("MAP VIEW -- Press M to Close");
                    while (Console.ReadKey().Key != ConsoleKey.M)
                    {
                        //Wait for map to be closed
                    }
                    break;
            }


        }
    }

    public int SetupDifficulty()
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