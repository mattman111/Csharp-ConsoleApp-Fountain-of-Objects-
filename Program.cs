using CSharpPlayersGuide.RichConsole;



//THIS IS THE ENTRY POINT
RichConsole.WriteLine("Welcome to Matt's Fountain of Objects!", TextEffects.DoubleUnderline);
RichConsole.WriteLine("Make sure to fullscreen! Double click the top bar! You may need to zoom out with CTRL + Mousewheel", Colors.Aqua, TextEffects.Blink);
Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
Console.SetWindowPosition(0, 0);


Game game = new Game();
// A new game has all data ready to go. 
game.SetupGame();

RichConsole.WriteLine($"Entrance: is at {game.GameGrid.Entrance.RoomInGridX} , {game.GameGrid.Entrance.RoomInGridY}. Player is here: {game.GameGrid.Entrance.PlayerPresent} ");

Player player1 = new Player();
RichConsole.WriteLine(player1.DisplayEmotionalState());

//Game

//TODO: 
// (1) Assign one entrance to a edge room.
// (1.5) Refactor Room Class
// (2) Draw 8 rooms around the players starting position [GAME VIEW]. (HOW DO I DO THIS?)



class Game
{

    public Difficulty difficulty;
    public Grid GameGrid;

    public Game()
    {
        // GRID CONSTRUCTOR
        GameGrid = new Grid(SetupDifficulty());
    }

    public void SetupGame()
    {
        GameGrid.DrawFullGrid();
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