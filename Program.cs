using CSharpPlayersGuide.RichConsole;



//THIS IS THE ENTRY POINT
RichConsole.WriteLine("Welcome to Matt's Fountain of Objects!", TextEffects.DoubleUnderline);
RichConsole.WriteLine("Make sure to fullscreen! Double click the top bar!", Colors.Aqua, TextEffects.Blink);
Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
Console.SetWindowPosition(0, 0);


Game game = new Game();
game.SetupGame();

RichConsole.WriteLine($"Entrance: is at {game.GameGrid.EdgeRooms[Array.FindIndex(game.GameGrid.EdgeRooms, index => index.RoomType == RoomType.Entrance)].RoomGridX} , {game.GameGrid.EdgeRooms[Array.FindIndex(game.GameGrid.EdgeRooms, index => index.RoomType == RoomType.Entrance)].RoomGridY} ");

Player player1 = new Player();
RichConsole.WriteLine(player1.DisplayEmotionalState());

//Game

//TODO: 
// (1) Assign one entrance to a edge room.
// (2) Draw 9 rooms from the players starting position. (HOW DO I DO THIS?)



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
            }
            RichConsole.WriteLine("Error: Not a valid input!", Colors.Red, TextEffects.Blink);
        }
        while (difficulty == Difficulty.Unset);
        return 0;
    }

}
