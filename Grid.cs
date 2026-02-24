using CSharpPlayersGuide.RichConsole;
using System.Numerics;

class Grid
{
    public int xlength { get; init; }
    public int ylength { get; init; }

    //Array of all room references
    public Room[,] CaveRooms { get; }

    //Array of edge room references (not two dimentional)
    public Room[] EdgeRooms { get; }

    //Array of non edge room references (not two dimentional)
    public Room[] NonEdgeRooms { get; }

    //Lists of all event rooms
    //I do not know the size of these until run time. They must be lists.
    public List<Room> PitRooms { get; }
    public List<Room> MaelstromRooms { get; }
    public List<Room> AmarokRooms { get; }

    public Room Entrance { get; private set; }

    public Room Fountain { get; private set; }


    public Room PlayerRoom { get; private set; }
    public Room[,] GameViewRooms { get; private set; }



    //TODO
    //Add a reference to the current player room


    //Grid Constants
    private int _entranceNum = 1;
    private int _fountainNum = 1;
    private int _minEventRooms;
    private int _maxEventRooms;


    public Grid(int gridsize)
    {
        this.xlength = gridsize;
        this.ylength = gridsize;
        _minEventRooms = gridsize;
        _maxEventRooms = (xlength * ylength) / 2;
        CaveRooms = new Room[xlength, ylength];
        EdgeRooms = new Room[(gridsize * gridsize) - ((int)Math.Pow(gridsize - 2, 2))];
        NonEdgeRooms = new Room[(int)Math.Pow(gridsize - 2, 2)];
        PitRooms = new List<Room>();
        MaelstromRooms = new List<Room>();
        AmarokRooms = new List<Room>();
        GameViewRooms = new Room[3,3];
        SetupGrid();
    }

    private EdgeDir GetEdgeDirection(int i, int j)
    {
        bool isTop = (i == 0);
        bool isBottom = (i == xlength - 1);
        bool isLeft = (j == 0);
        bool isRight = (j == ylength - 1);

        if (isTop && isLeft) return EdgeDir.NW;
        if (isTop && isRight) return EdgeDir.NE;
        if (isBottom && isLeft) return EdgeDir.SW;
        if (isBottom && isRight) return EdgeDir.SE;
        if (isTop) return EdgeDir.N;
        if (isBottom) return EdgeDir.S;
        if (isLeft) return EdgeDir.W;
        if (isRight) return EdgeDir.E;

        return EdgeDir.None;
    }
    public void SetupGrid()
    {
        SetRoomsInsideOutside();
        SetRoomTypes();
        RefreshGameView();
    }

    private void SetRoomsInsideOutside()
    {

        for (int i = 0; i < xlength; i++)
        {
            for (int j = 0; j < ylength; j++)
            {

                CaveRooms[i, j] = new Room(i, j);
                EdgeDir dir = GetEdgeDirection(i, j);

                if (dir != EdgeDir.None)
                {
                    int firstEmpty = Array.IndexOf(EdgeRooms, null);
                    CaveRooms[i, j].EdgeDirection = dir;
                    EdgeRooms[firstEmpty] = (CaveRooms[i, j]);
                }
                else
                {
                    int firstEmpty = Array.IndexOf(NonEdgeRooms, null);
                    NonEdgeRooms[firstEmpty] = (CaveRooms[i, j]);
                }
            }
        }
    }

    private void SetRoomTypes()
    {
        // Assign Entrance
        for (int i = 0; i < _entranceNum; i++)
        {
            var entranceroom = EdgeRooms[Random.Shared.Next(0, EdgeRooms.Length)];
            entranceroom.SetRoomType(RoomType.Entrance);
            entranceroom.PlayerPresent = true;
            entranceroom.RoomStatus = RoomStatus.Known;
            entranceroom.PlayerPosition = new Vector2(2, 2);
            PlayerRoom = entranceroom;
            Entrance = entranceroom;
        }

        // Assign Fountain
        for (int i = 0; i < _fountainNum; i++)
        {
            var fountainroom = NonEdgeRooms[Random.Shared.Next(0, NonEdgeRooms.Length)];
            fountainroom.SetRoomType(RoomType.Fountain);
            Fountain = fountainroom;
        }

        //Assign Event Rooms
        int actualNumOfEventRooms = Random.Shared.Next(_minEventRooms, _maxEventRooms);
        while ((PitRooms.Count + MaelstromRooms.Count + AmarokRooms.Count) < actualNumOfEventRooms)
        {
            int x = Random.Shared.Next(0, xlength);
            int y = Random.Shared.Next(0, ylength);

            if (CaveRooms[x, y].RoomType == RoomType.Empty)
            {
                Room room = CaveRooms[x, y];

                switch (Random.Shared.Next(1, 4))
                {
                    case 1:
                        CaveRooms[x, y].SetRoomType(RoomType.Pit);
                        PitRooms.Add(room);
                        break;
                    case 2:
                        CaveRooms[x, y].SetRoomType(RoomType.Maelstrom);
                        MaelstromRooms.Add(room);
                        break;
                    case 3:
                        CaveRooms[x, y].SetRoomType(RoomType.Amarok);
                        AmarokRooms.Add(room);
                        break;
                }

            }

        }
    }

    /// <summary>
    /// This method provides a 2d array of nine rooms with the player's room being centered in the array. 
    /// This method is fucked and idk how to fix it. All I can do is pray for forgiveness. 
    /// </summary>
    public void RefreshGameView()
    {
        int px = PlayerRoom.RoomInGridX;
        int py = PlayerRoom.RoomInGridY;

        // Loop through y -1,0, and 1 to get a 3x3 space around the player at the center of the 3x3 space.
        // There has to be a better way to do this. 
        for (int yOffset = -1; yOffset <= 1; yOffset++)
        {
            // Do the same for x
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                int targetX = px + xOffset;
                int targetY = py + yOffset;

                // Check array bounderies per dimenion
                if (targetX > -1 && targetX < CaveRooms.GetLength(0) &&
                    targetY > -1 && targetY < CaveRooms.GetLength(1))
                {
                    // Correct offset for array assignment
                    GameViewRooms[xOffset + 1, yOffset + 1] = CaveRooms[targetX, targetY];
                }
                else
                {
                    // Room is out of bounds and should be null in here
                    GameViewRooms[xOffset + 1, yOffset + 1] = null;
                }
            }
        }
    }

    /// <summary>
    /// This method will draw the entire known game grid
    /// </summary>
    public void DrawFullMap()
    {
        //Grid Row
        for (int i = 0; i < xlength; i++)
        {
            //Room Row
            for (int j = 0; j < Room.NumOfRoomTilesInRow; j++)
            {
                // Grid Col
                for (int k = 0; k < ylength; k++)
                {
                    CaveRooms[i, k].DrawRoomRows(j);
                }
                RichConsole.Write("\n");
            }
        }
    }

    public void DrawGameView()
    {
        //Grid Row
        for (int i = 0; i < GameViewRooms.GetLength(0); i++)
        {
            //Room Row
            for (int j = 0; j < Room.NumOfRoomTilesInRow; j++)
            {
                // Grid Col
                for (int k = 0; k < GameViewRooms.GetLength(1); k++)
                {
                    if (GameViewRooms[i, k] != null)
                    {
                        GameViewRooms[i, k].DrawRoomRows(j);
                    }
                    else
                    {
                        RichConsole.Write("          ");
                    }
                }
                RichConsole.Write("\n");
            }
        }
    }
}