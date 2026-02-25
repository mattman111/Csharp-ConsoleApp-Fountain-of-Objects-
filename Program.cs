using CSharpPlayersGuide.RichConsole;
using System.Numerics;


//THIS IS THE ENTRY POINT
RichConsole.WriteLine("  ______                _        _          ____   __    ____  _     _           _        \r\n" +
    " |  ____|              | |      (_)        / __ \\ / _|  / __ \\| |   (_)         | |       \r\n" +
    " | |__ ___  _   _ _ __ | |_ __ _ _ _ __   | |  | | |_  | |  | | |__  _  ___  ___| |_ ___  \r\n" +
    " |  __/ _ \\| | | | '_ \\| __/ _` | | '_ \\  | |  | |  _| | |  | | '_ \\| |/ _ \\/ __| __/ __| \r\n" +
    " | | | (_) | |_| | | | | || (_| | | | | | | |__| | |   | |__| | |_) | |  __/ (__| |_\\__ \\ \r\n" +
    " |_|  \\___/ \\__,_|_| |_|\\__\\__,_|_|_| |_|  \\____/|_|    \\____/|_.__/| |\\___|\\___|\\__|___/ \r\n" +
    " By: Matt                                                         __/ |                   \r\n" +
    " With tons of help from the Byte Club                             |___/                    ", Colors.AntiqueWhite, TextEffects.Blink);
RichConsole.WriteLine("\nMake sure to fullscreen! Double click the top bar! You may need to adjust zoom level with LCTRL + Mousewheel", Colors.Aqua, TextEffects.Blink);
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
    public List<Message> PlayerMessages { get; } = new List<Message>();

    public Game()
    {
        // GRID CONSTRUCTOR
        GameGrid = new Grid(SetupDifficulty());
        player = new Player();
    }

    public void StartGame()
    {
        //Starting Message
        PlayerMessages.Add(new Message("You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects.\n" +
            "Light is visible only in the entrance, and no other light is seen anywhere in the caverns.\n" +
            "You must navigate the Caverns with your other senses.\n" +
            "Find the Fountain of Objects, activate it, and return to the entrance. You may get lost, I have provided a magic map. Use it with M.\n" +
            "I believe in you, adventurer. Use your arrow keys to begin.\n", Colors.White, TextEffects.Underline));

        //GAME LOOP
        while (true)
        {
            //Clear the screen
            RichConsole.Clear();

            //Update and draw the game view
            GameGrid.DrawGameView();

            //Prepare all necessary player messages
            PreparePlayerMessages();

            //UI TEXT
            #region
            RichConsole.WriteLine("-- GAME VIEW --");
            RichConsole.WriteLine("CONTROLS - SPACE TO PREPARE BOW - H FOR HELP - ESC FOR QUIT");
            RichConsole.WriteLine($"PLAYER's X: {player.x} Y: {player.y} ROOM X={GameGrid.PlayerRoom.RoomInGridX},Y={GameGrid.PlayerRoom.RoomInGridY}");
            RichConsole.WriteLine($"PLAYER's Arrow Count: \n");
            for (int i = 0; i < PlayerMessages.Count; i++)
            {
                RichConsole.WriteLine(PlayerMessages[i].message, PlayerMessages[i].color, PlayerMessages[i].effect);
            }
            PlayerMessages.Clear();
            RichConsole.WriteLine("\n");
            #endregion


            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                    RichConsole.Write("BOW ARMED - CHOOSE A DIRECTION - SPACEBAR TO DISENAGE BOW - ARROW KEYS TO FIRE");
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
                    RichConsole.WriteLine("-- MAP VIEW --\nPress M to Close");
                    while (Console.ReadKey().Key != ConsoleKey.M)
                    {
                        //Wait for map to be closed
                    }
                    Console.Beep(100, 1);
                    break;
            }

            // GAMEPLAY LOGIC
            HandleGameplay();

        }
    }

    private void HandleGameplay()
    {
        var playerroom = GameGrid.PlayerRoom;
        var roomtype = GameGrid.PlayerRoom.RoomType;
       
        //TODO
        // GET WAY TO CHECK PLAYER TILE DATA

        if (roomtype == null) return;
        switch (roomtype)
        {
            case RoomType.Maelstrom:
                GameGrid.AssignNewPlayerRoom(GameGrid.CaveRooms[Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(0)), Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(1))], 2, 2);
                playerroom.RoomType = RoomType.Empty;
                playerroom.RemoveEntity(2, 2, new Entity(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink));
                PlayerMessages.Add(new Message("🌪️ A maelstrom has fluttered you away to an unknown room. You must find your bearings.. 🌪️", Colors.Yellow, TextEffects.Blink));
                Console.Beep(1000, 100);
                break;

        }
    }

    private void PreparePlayerMessages()
    {
        for(int i = 0; i < GameGrid.GameViewRooms.GetLength(0); i++)
        {
            for (int j = 0; j < GameGrid.GameViewRooms.GetLength(1); j++)
            {
                if (GameGrid.GameViewRooms[i, j] != null)
                {
                    var room = GameGrid.GameViewRooms[i, j];
                    switch (room.RoomType)
                    {
                        case RoomType.Entrance:
                            PlayerMessages.Add(new Message("🍂 Autumn wind can be heard stiring in the distance.. 🍂", Colors.Orange, TextEffects.Italics));
                            break;
                        case RoomType.Amarok:
                            PlayerMessages.Add(new Message("🐺 You can smell the rotten stench of an amarok in a nearby room.. 🐺", Colors.Red, TextEffects.Blink));
                            break;
                        case RoomType.Pit:
                            PlayerMessages.Add(new Message("🕳️ You feel a draft. There is a pit in a nearby room.. 🕳️", Colors.LightGray, TextEffects.Italics));
                            break;
                        case RoomType.Maelstrom:
                            PlayerMessages.Add(new Message("🌪️ You hear the growling and groaning of a maelstrom nearby.. 🌪️", Colors.Yellow, TextEffects.Italics));
                            break;
                        case RoomType.Fountain:
                            PlayerMessages.Add(new Message("⛲ You hear running water.. ⛲", Colors.Aqua, TextEffects.Blink));
                            break;
                    }

                }
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
            HandleRoomTransition(targetX, targetY);
            return;
        }

        // Check for Solid Walls
        if (GameGrid.PlayerRoom.TilesData[targetX, targetY].TileType == TileType.Solid)
        {
            Console.Beep(7000, 1);
            return;
        }

        // Update Position
        player.x = targetX;
        player.y = targetY;
        GameGrid.PlayerRoom.PlayerPosition = new Vector2(targetX, targetY);
        Console.Beep(4000, 1);
    }

    private void HandleRoomTransition(int targetX, int targetY)
    {
        int newRoomGridX = GameGrid.PlayerRoom.RoomInGridX;
        int newRoomGridY = GameGrid.PlayerRoom.RoomInGridY;

        //Adjust Grid coordinates and wrap player local coordinates
        // Moving Up
        if (targetX < 0) 
        {
            newRoomGridX--;
            player.x = 4;
        }
        // Moving Down
        else if (targetX > 4) 
        {
            newRoomGridX++;
            player.x = 0;
        }
        // Moving Left
        if (targetY < 0) 
        {
            newRoomGridY--;
            player.y = 4;
        }
        // Moving Right
        else if (targetY > 4)
        {
            newRoomGridY++;
            player.y = 0;
        }

        // Confirm we are in bounds
        if (newRoomGridX >= 0 && newRoomGridX < GameGrid.CaveRooms.GetLength(0) && newRoomGridY >= 0 && newRoomGridY < GameGrid.CaveRooms.GetLength(1))
        {
            GameGrid.AssignNewPlayerRoom(GameGrid.CaveRooms[newRoomGridX, newRoomGridY], player.x, player.y);
            Console.Beep(2000, 10);
        }
        else
        {
            // Clamp player's position
            PlayerMessages.Add(new Message("⛲ An shimmering breeze whisks you back into the shadows. Your hour has not yet struck... the Fountain still sleeps. ⛲", Colors.Aqua, TextEffects.Blink));
            player.x = Math.Clamp(player.x, 0, 4);
            player.y = Math.Clamp(player.y, 0, 4);
        }
    }

    private int SetupDifficulty()
    {
        do
        {
            RichConsole.WriteLine($"\nPossible Difficulty Options Include - test, easy, medium, hard, insane, and hurtme");
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
            Console.Beep(100, 100);
        }
        while (difficulty == Difficulty.Unset);
        return 0;
    }
}

public record Message(string message, Color color, TextEffects effect);