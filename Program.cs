using CSharpPlayersGuide.RichConsole;
using System.Numerics;


//THIS IS THE ENTRY POINT


//Primary loop
do
{
    RichConsole.WriteLine("  ______                _        _          ____   __    ____  _     _           _        \r\n" +
    " |  ____|              | |      (_)        / __ \\ / _|  / __ \\| |   (_)         | |       \r\n" +
    " | |__ ___  _   _ _ __ | |_ __ _ _ _ __   | |  | | |_  | |  | | |__  _  ___  ___| |_ ___  \r\n" +
    " |  __/ _ \\| | | | '_ \\| __/ _` | | '_ \\  | |  | |  _| | |  | | '_ \\| |/ _ \\/ __| __/ __| \r\n" +
    " | | | (_) | |_| | | | | || (_| | | | | | | |__| | |   | |__| | |_) | |  __/ (__| |_\\__ \\ \r\n" +
    " |_|  \\___/ \\__,_|_| |_|\\__\\__,_|_|_| |_|  \\____/|_|    \\____/|_.__/| |\\___|\\___|\\__|___/ \r\n" +
    " By: Matt                                                         __/ |                   \r\n" +
    " With tons of help from the Byte Club                             |___/                    ", Colors.AntiqueWhite, TextEffects.None);
    RichConsole.WriteLine("\nMake sure to fullscreen! Double click the top bar! You may need to adjust zoom level with LCTRL + Mousewheel", Colors.Aqua, TextEffects.None);
    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
    Console.SetWindowPosition(0, 0);
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Black;
    Game game = new Game();
    // A new game has all data ready to go. 
    game.PlayGame();
    RichConsole.Clear();
} while (true);





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

    public void PlayGame()
    {
        //Starting Message
        PlayerMessages.Add(new Message("You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects.\n" +
            "Light is visible only in the entrance, and no other light is seen anywhere in the caverns.\n" +
            "You must navigate the Caverns with your other senses.\n" +
            "Find the Fountain of Objects, activate it, and return to the entrance. You may get lost, I have provided a magic map. Use it with M.\n" +
            "I believe in you, adventurer. Use your arrow keys to begin.\n", Colors.White, TextEffects.None));

        //GAME LOOP
        while (player.IsAlive == true)
        {
            //Clear the screen
            RichConsole.Clear();

            //Update and draw the game view
            GameGrid.DrawGameView();

            //Prepare all necessary player messages
            PreparePlayerMessages();

            #region UI_TEXT
            DisplayUI();
            #endregion


            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                    RichConsole.Write("BOW ARMED - CHOOSE A DIRECTION - SPACEBAR TO DISENAGE BOW - ARROW KEYS TO FIRE");
                    var key2 = Console.ReadKey(true);
                    switch (key2.Key)
                    {
                        case ConsoleKey.UpArrow:
                            HandleArrowShot(CardinalDir.North);
                            break;
                        case ConsoleKey.DownArrow:
                            HandleArrowShot(CardinalDir.South);
                            break;
                        case ConsoleKey.RightArrow:
                            HandleArrowShot(CardinalDir.East);
                            break;
                        case ConsoleKey.LeftArrow:
                            HandleArrowShot(CardinalDir.West);
                            break;
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
                    Console.Beep(100, 1);
                    RichConsole.Clear();
                    GameGrid.DrawFullMap();
                    RichConsole.WriteLine("-- MAP VIEW --\nPress M to Close");
                    while (Console.ReadKey().Key != ConsoleKey.M)
                    {
                        //Wait for map to be closed
                    }
                    Console.Beep(100, 1);
                    break;
                case ConsoleKey.H:
                    RichConsole.Clear();
                    RichConsole.WriteLine($" -- HELP VIEW --\n" +
                        $"Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a pit, you will die.\n" +
                        $"Maelstroms are violent forces of sentient wind. Entering a room with one could transport you to any other location in the caverns. You will be able to hear their growling and groaning in nearby rooms.\n" +
                        $"Amaroks roam the caverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.\n" +
                        $"You carry with you a bow and a quiver of arrows. You can use them to shoot monsters in the caverns but be warned: you have a limited supply.\n\n" +
                        $"CONTROLS - ARROW KEYS TO MOVE - SPACE TO PREPARE BOW\n" +
                        $" -- END OF HELP VIEW --\n" +
                        $" PRESS ESCAPE TO RETURN");
                    while (Console.ReadKey().Key != ConsoleKey.Escape)
                    {
                        //Wait for help to be closed
                    }
                    break;
            }

            // GAMEPLAY LOGIC
            HandleGameplay();

        }
    }

    private void HandleArrowShot(CardinalDir shotdir)
    {
        // Check arrow count
        if (player.ArrowCount <= 0)
        {
            PlayerMessages.Add(new Message("🏹 You reach for an arrow, but your quiver is empty! 🏹", Colors.SandyBrown, TextEffects.None));
            return;
        }
        player.ArrowCount--;

        // Determine current room cords
        int targetRoomX = GameGrid.PlayerRoom.RoomInGridX;
        int targetRoomY = GameGrid.PlayerRoom.RoomInGridY;
        Room currentRoom = GameGrid.PlayerRoom;

        // Calculate target room cords with shotdir
        switch (shotdir)
        {
            case CardinalDir.North:
                targetRoomX--;
                break;
            case CardinalDir.South:
                targetRoomX++;
                break;
            case CardinalDir.West:
                targetRoomY--;
                break;
            case CardinalDir.East:
                targetRoomY++;
                break;
        }

        // Check the shot is in a valid spot
        if (ReturnValidShootPosition(player.x, player.y, shotdir))
        {
            // Check if the target room exists within the grid
            if (targetRoomX >= 0 && targetRoomX < GameGrid.CaveRooms.GetLength(0) && targetRoomY >= 0 && targetRoomY < GameGrid.CaveRooms.GetLength(1))
            {
                Room targetRoom = GameGrid.CaveRooms[targetRoomX, targetRoomY];

                if (targetRoom.RoomType == RoomType.Amarok)
                {
                    PlayerMessages.Add(new Message($"🏹 Your arrow flies {shotdir} and strikes a foul beast! You hear a dying scream. 🏹", Colors.SandyBrown, TextEffects.None));

                    // Kill the monster
                    targetRoom.RoomType = RoomType.Empty;
                    targetRoom.RemoveEntity(2, 2, new Entity(TileType.Amarok, TileColor.Red, TileEffect.Blink));
                }
                else if (targetRoom.RoomType == RoomType.Maelstrom)
                {
                    // Calculate teleported arrow
                    int teleportArrowx = Random.Shared.Next(0, GameGrid.xlength);
                    int teleportArrowy = Random.Shared.Next(0, GameGrid.ylength);
                    Room teleportArrowRoom = GameGrid.CaveRooms[teleportArrowx, teleportArrowy];

                    if (teleportArrowRoom.RoomType == RoomType.Amarok)
                    {
                        PlayerMessages.Add(new Message($"🏹 You fire {shotdir} into a Maelstrom. You hear a dying scream echoing in the distance. 🏹", Colors.SandyBrown, TextEffects.None));
                        teleportArrowRoom.RoomType = RoomType.Empty;
                        teleportArrowRoom.RemoveEntity(2, 2, new Entity(TileType.Amarok, TileColor.Red, TileEffect.Blink));
                    }
                    else
                    {
                        PlayerMessages.Add(new Message($"🏹 You fire {shotdir} into a Maelstrom. The arrow vanishes into the whirling storm... and hits nothing. 🏹", Colors.SandyBrown, TextEffects.None));
                    }

                    //Move Maelstrom
                    targetRoom.RoomType = RoomType.Empty;
                    targetRoom.RemoveEntity(2, 2, new Entity(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink));
                    PlayerMessages.Add(new Message("🌪️ A nearby maelstrom has sputtered out. It has gone someplace else..  🌪️", Colors.Yellow, TextEffects.Blink));
                    bool newMaelstromAssigned = false;
                    while (newMaelstromAssigned == false)
                    {
                        int newMaelstromX = Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(0));
                        int newMaelstromY = Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(1));
                        Room targetroom = GameGrid.CaveRooms[newMaelstromX, newMaelstromY];
                        if (targetroom.RoomType == RoomType.Empty)
                        {
                            targetroom.RoomType = RoomType.Maelstrom;
                            targetroom.AddEntity(2, 2, new Entity(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink));

                            newMaelstromAssigned = true;
                        }
                    }
                }
                else
                {
                    PlayerMessages.Add(new Message($"🏹 You fire {shotdir}. The arrow vanishes into the darkness... and hits nothing. 🏹", Colors.SandyBrown, TextEffects.None));
                }
            }
            else
            {
                //Check if player fires arrow out of the entrance like a werido.
                if (currentRoom == GameGrid.Entrance)
                {
                    PlayerMessages.Add(new Message($"🏹 You fire an arrow {shotdir} outside, and it flys away! 🏹", Colors.SandyBrown, TextEffects.None));
                }
            }
        }
        else
        {

            PlayerMessages.Add(new Message($"🏹 You fire an arrow {shotdir}, but it shatters against a nearby wall! 🏹", Colors.SandyBrown, TextEffects.None));
        }
    }

    private bool ReturnValidShootPosition(int x, int y, CardinalDir shotdir)
    {
        switch (shotdir)
        {
            case CardinalDir.North:
            case CardinalDir.South:
                if (y == 2) return true;
                break;
            case CardinalDir.West:
            case CardinalDir.East:
                if (x == 2) return true;
                break;
        }
        return false;
    }

    private void HandleGameplay()
    {
        var playerroom = GameGrid.PlayerRoom;
        var roomtype = GameGrid.PlayerRoom.RoomType;

        if (roomtype == null) return;
        switch (roomtype)
        {
            case RoomType.Maelstrom:
                GameGrid.AssignNewPlayerRoom(GameGrid.CaveRooms[Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(0)), Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(1))], 2, 2);
                playerroom.RoomType = RoomType.Empty;
                playerroom.RemoveEntity(2, 2, new Entity(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink));
                PlayerMessages.Add(new Message("🌪️ A maelstrom has fluttered you away to an unknown room. You must find your bearings.. 🌪️", Colors.Yellow, TextEffects.Blink));
                bool newMaelstromAssigned = false;
                while (newMaelstromAssigned == false)
                {
                    int newMaelstromX = Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(0));
                    int newMaelstromY = Random.Shared.Next(0, GameGrid.CaveRooms.GetLength(1));
                    Room targetroom = GameGrid.CaveRooms[newMaelstromX, newMaelstromY];
                    if (targetroom.RoomType == RoomType.Empty)
                    {
                        targetroom.RoomType = RoomType.Maelstrom;
                        targetroom.AddEntity(2, 2, new Entity(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink));

                        newMaelstromAssigned = true;
                    }
                }
                Console.Beep(100, 100);
                break;
            case RoomType.Amarok:
                HandleTeardown("🐺 An Amarok leaps for you with saliva-dripping teeth. You know your end is now.. 🐺", Colors.Red);
                break;
        }
    }

    private void HandleTeardown(string finalmessage, Color finalmessagecolor)
    {
        PlayerMessages.Clear();
        PlayerMessages.Add(new Message(finalmessage, finalmessagecolor, TextEffects.None));
        PlayerMessages.Add(new Message("Your time in the Cavern of Objects has come to an end. Press Escape to return to main menu.", Colors.White, TextEffects.DoubleUnderline));
        RichConsole.Clear();
        GameGrid.DrawFullMap();
        RichConsole.WriteLine("-- Final Discovered Map --\n" +
            "Press Escape to return to menu.");
        DisplayPlayerMessages();
        Console.Beep(2000, 100);
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
            //Wait for player to hit escape
        }
        player.SetPlayerLivingStatus(false);
    }

    private void DisplayUI()
    {
        RichConsole.WriteLine("-- GAME VIEW --");
        RichConsole.WriteLine("CONTROLS - SPACE TO PREPARE BOW - H FOR HELP - ESC FOR QUIT");
        RichConsole.WriteLine($"PLAYER's X: {player.x} Y: {player.y} ROOM X={GameGrid.PlayerRoom.RoomInGridX},Y={GameGrid.PlayerRoom.RoomInGridY}");
        RichConsole.WriteLine($"PLAYER's Arrow Count: {player.ArrowCount} \n");
        DisplayPlayerMessages();
        RichConsole.WriteLine("\n");
    }

    private void DisplayPlayerMessages()
    {
        for (int i = 0; i < PlayerMessages.Count; i++)
        {
            RichConsole.WriteLine(PlayerMessages[i].message, PlayerMessages[i].color, PlayerMessages[i].effect);
        }
        PlayerMessages.Clear();
    }

    private void PreparePlayerMessages()
    {

        if (player.IsAlive == true)
        {
            //GameViewRooms Messages
            for (int i = 0; i < GameGrid.GameViewRooms.GetLength(0); i++)
            {
                for (int j = 0; j < GameGrid.GameViewRooms.GetLength(1); j++)
                {
                    if (GameGrid.GameViewRooms[i, j] != null)
                    {
                        var room = GameGrid.GameViewRooms[i, j];
                        var playerroom = GameGrid.PlayerRoom;
                        switch (room.RoomType)
                        {
                            case RoomType.Entrance:
                                PlayerMessages.Add(new Message("🍂 Autumn wind can be heard stirring in the distance.. 🍂", Colors.Orange, TextEffects.Italics));
                                break;
                            case RoomType.Amarok:
                                PlayerMessages.Add(new Message("🐺 You can smell the rotten stench of an amarok in a nearby room.. 🐺", Colors.Red, TextEffects.None));
                                break;
                            case RoomType.Pit:
                                if (room.PlayerPresent == true) PlayerMessages.Add(new Message("🕳️ You feel strong wind coming up. There must be pits in this room.. 🕳️", Colors.LightGray, TextEffects.Italics));
                                if (room.PlayerPresent == false) PlayerMessages.Add(new Message("🕳️ You feel a draft. There is a pit in a nearby room.. 🕳️", Colors.LightGray, TextEffects.Italics));
                                break;

                            case RoomType.Maelstrom:
                                PlayerMessages.Add(new Message("🌪️ You hear the growling and groaning of a maelstrom nearby.. 🌪️", Colors.Yellow, TextEffects.Italics));
                                break;
                            case RoomType.Fountain:
                                if (room.PlayerPresent == true) PlayerMessages.Add(new Message("⛲ Your senses are overwhelmed with the sound of running water..  ⛲", Colors.Aqua, TextEffects.None));
                                if (room.PlayerPresent == false) PlayerMessages.Add(new Message("⛲ You hear running water nearby.. ⛲", Colors.Aqua, TextEffects.None));
                                break;
                        }

                    }
                }
            }
        }


        //Victory Messages
        if (player.HasWon == true)
        {
            PlayerMessages.Add(new Message("⛲ The cavern rumbles with magic. The Fountain has been activated! Hurry to the exit! ⛲", Colors.Aqua, TextEffects.None));
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

        //Check for pits and then kill the player
        if ((GameGrid.PlayerRoom.TilesData[targetX, targetY].TileType) == TileType.Pit)
        {
            player.x = targetX;
            player.y = targetY;
            GameGrid.PlayerRoom.PlayerPosition = new Vector2(targetX, targetY);
            HandleTeardown("🕳️ Your footing slips and you fall into a pit. You know your end is now.. 🕳️", Colors.LightGray);
        }

        //Check for fountain
        if ((GameGrid.PlayerRoom.TilesData[targetX, targetY].TileType) == TileType.Fountain)
        {
            player.HasWon = true;
        }

        if ((GameGrid.PlayerRoom.TilesData[targetX, targetY].TileType) == TileType.Arrow)
        {
            player.x = targetX;
            player.y = targetY;
            GameGrid.PlayerRoom.PlayerPosition = new Vector2(targetX, targetY);
            GameGrid.PlayerRoom.RoomType = RoomType.Empty;
            GameGrid.PlayerRoom.RemoveEntity(targetX, targetY, new Entity(TileType.Arrow, TileColor.Brown, TileEffect.Blink));
            PlayerMessages.Add(new Message("🏹 You find an old bow and arrow. Only the arrow is salvageable. Your arrow count has increased.. 🏹", Colors.SandyBrown, TextEffects.None));
            player.ArrowCount += 1;
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
            Console.Beep(2000, 5);
        }
        else
        {
            // Clamp player's position
            if (player.HasWon == true)
            {
                HandleTeardown("You exit the cavern with the fountain activated. Your quest has come to an end..", Colors.White);
            }
            else
            {
                PlayerMessages.Add(new Message("🍂 An shimmering breeze whisks you back into the shadows. Your hour has not yet struck... the Fountain still sleeps.. 🍂", Colors.Orange, TextEffects.Italics));
            }
            player.x = 2;
            player.y = 2;
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
            RichConsole.WriteLine("Error: Not a valid input!", Colors.Red, TextEffects.None);
            Console.Beep(100, 100);
        }
        while (difficulty == Difficulty.Unset);
        return 0;
    }
}

public record Message(string message, Color color, TextEffects effect);
public enum CardinalDir { North, South, East, West }