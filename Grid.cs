using CSharpPlayersGuide.RichConsole;

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

    //Grid Constants
    private int _entranceNum = 1;
    private int _fountainNum = 1;
    private int _minEventRooms = 1;

    //       Total   Edge   Inside 
    // 3x3 =   9      8       1
    // 4x4 =   16     12      4     
    // 5x5 =   25     16      9
    // Edge rooms = total - inside#
    // NonEdge rooms = inside


    public Grid(int gridsize)
    {
        this.xlength = gridsize;
        this.ylength = gridsize;
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
        SetRoomTypes(_entranceNum, _fountainNum, _minEventRooms);
    }

    private void SetRoomsInsideOutside()
    {

        for (int i = 0; i < xlength; i++)
        {
            for (int j = 0; j < ylength; j++)
            {

                CaveRooms[i, j] = new Room(i, j);
                EdgeDir? dir = GetEdgeDirection(i, j);

                if (dir != EdgeDir.NOEDGE)
                {
                    int firstEmpty = Array.IndexOf(EdgeRooms, null);
                    CaveRooms[i, j].SetRoomAsEdgeRoom(dir.Value);
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

    private void SetRoomTypes(int entranceNum, int fountainNum, int minEventRooms)
    {
        // Assign Entrance
        Random RandomEntrance = new Random();
        for (int i = 0; i < entranceNum; i++)
        {
            EdgeRooms[RandomEntrance.Next(0, EdgeRooms.Length)].SetRoomType(RoomType.Entrance);
        }

        // Assign Fountain
        Random RandomFountain = new Random();
        for (int i = 0; i < fountainNum; i++)
        {
            NonEdgeRooms[RandomFountain.Next(0, EdgeRooms.Length)].SetRoomType(RoomType.Fountain);
        }
    }

    /// <summary>
    /// This method will draw the entire game grid. 
    /// </summary>
    public void DrawFullGrid()
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