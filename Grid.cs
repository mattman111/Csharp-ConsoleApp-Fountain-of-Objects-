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

    //TODO
    //Add array to all event rooms

    public Room Entrance { get; private set; }

    public Room Fountain {  get; private set; }

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

        return EdgeDir.NOEDGE;
    }
    public void SetupGrid()
    {
        SetRoomsInsideOutside();
        SetRoomTypes();
    }

    private void SetRoomsInsideOutside()
    {

        for (int i = 0; i < xlength; i++)
        {
            for (int j = 0; j < ylength; j++)
            {

                CaveRooms[i, j] = new Room(i, j);
                EdgeDir dir = GetEdgeDirection(i, j);

                if (dir != EdgeDir.NOEDGE)
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
            entranceroom.PlayerPosition = new Vector2(2,2);
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
        for (int i = 0; i < Random.Shared.Next(_minEventRooms, _maxEventRooms); i++)
        {
            int x = Random.Shared.Next(0, xlength);
            int y = Random.Shared.Next(0, ylength);
            if (CaveRooms[x, y].RoomType == RoomType.Empty)
            {
                switch (Random.Shared.Next(1, 4))
                {
                    case 1:
                        CaveRooms[x, y].SetRoomType(RoomType.Pit);
                        break;
                    case 2:
                        CaveRooms[x, y].SetRoomType(RoomType.Maelstrom);
                        break;
                    case 3:
                        CaveRooms[x, y].SetRoomType(RoomType.Amarok);
                        break;
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
}