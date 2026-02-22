using CSharpPlayersGuide.RichConsole;

class Room
{
    private int _xlength = 5;
    private int _ylength = 5;
    public static int RowNum;

    //It seems like Tiles should be a class?
    public TileTypes[,] tileTypeData { get; init; }
    public TileColor[,] tileColorData { get; init; }

    public EdgeDir EdgeDirection { get; private set; }

    public RoomType RoomType { get; private set; }
    public RoomStatus RoomStatus { get; private set; }
    public RoomEvents Events { get; private set; }

    public Room()
    {
        tileTypeData = new TileTypes[_xlength, _ylength];
        tileColorData = new TileColor[_xlength, _ylength];
        RowNum = _xlength;
        EdgeDirection = EdgeDir.NOEDGE;
        RoomType = RoomType.EventRoom;
        RoomStatus = RoomStatus.Unknown;
        Events = RoomEvents.None;
        SetupRoom();
    }

    public void SetupRoom()
    {
        for (int i = 0; i < _xlength; i++)
        {
            for (int j = 0; j < _ylength; j++)
            {
                if (i == 0 && j == 0 || i == 0 && j == 1 || i == 0 && j == 3 || i == 0 && j == 4
                    || i == 1 && j == 0 || i == 1 && j == 4
                    || i == 3 & j == 0 || i == 3 && j == 4
                    || i == 4 && j == 0 || i == 4 && j == 1 || i == 4 && j == 3 || i == 4 && j == 4)
                {
                    tileTypeData[i, j] = TileTypes.Solid;
                    tileColorData[i, j] = TileColor.DarkGrey;
                }
                else
                {
                    tileTypeData[i, j] = TileTypes.Empty;
                    tileColorData[i, j] = TileColor.Black;
                }

            }
        }
    }

    public void DrawRoomRows(int row)
    {
        for (int i = 0; i < _xlength; i++)
        {
            DrawTile(row, i);
        }
    }

    private void DrawTile(int x, int y)
    {
        RichConsole.Write(GetTileArt(x, y), GetTileColor(x, y), TextEffects.None);
    }

    public Color GetTileColor(int x, int y)
    {
        switch (tileColorData[x, y])
        {
            case TileColor.LightGrey:
                return Colors.LightGray;
            case TileColor.Grey:
                return Colors.Gray;
            case TileColor.DarkGrey:
                return Colors.DimGray;
            case TileColor.Blue:
                return Colors.Blue;
            default:
                return Colors.Red;
        }
    }

    public void SetTileType(int x, int y, TileTypes tiletype)
    {
        tileTypeData[x, y] = tiletype;
    }

    public string GetTileArt(int x, int y)
    {
        switch (tileTypeData[x, y])
        {
            case TileTypes.Empty:
                return "  ";
            case TileTypes.Solid:
                return "🧱";
            case TileTypes.Entrance:
                return "⛆";
            case TileTypes.Player:
                return "😐";
            default:
                return "?";
        }
    }

    public TileColor ReturnRoomColor()
    {
        switch (RoomType)
        {
            case RoomType.Entrance:
                return TileColor.White;
            case RoomType.EventRoom:
                return TileColor.DarkGrey;
            default:
                return TileColor.Blue;
        }
    }

    public TileColor ReturnRoomStatusColor()
    {
        switch (RoomStatus)
        {
            case RoomStatus.Unknown:
                return TileColor.Black;
            case RoomStatus.Known:
                return TileColor.DarkGrey;
            default:
                return TileColor.Blue;
        }
    }

    public void SetRoomColor(TileColor tilecolor)
    {
        for (int i = 0; i < _xlength; i++)
        {
            for (int j = 0; j < _ylength; j++)
            {
                if (i == 0 && j == 0 || i == 0 && j == 1 || i == 0 && j == 3 || i == 0 && j == 4
                    || i == 1 && j == 0 || i == 1 && j == 4
                    || i == 3 && j == 0 || i == 3 && j == 4
                    || i == 4 && j == 0 || i == 4 && j == 1 || i == 4 && j == 3 || i == 4 && j == 4)
                {
                    tileTypeData[i, j] = TileTypes.Solid;
                    tileColorData[i, j] = tilecolor;
                }
                else
                {
                    tileTypeData[i, j] = TileTypes.Empty;
                    tileColorData[i, j] = TileColor.Black;
                }

            }
        }
    }

    public void SetRoomAsEdgeRoom(EdgeDir edgedir)
    {
        switch (edgedir)
        {
            case EdgeDir.NOEDGE:
                EdgeDirection = edgedir;
                break;
            default:
                EdgeDirection = edgedir;
                break;
        }

        RefreshRoomStates();
        //Edit tiles
    }


    private void RefreshRoomStates()
    {
        //These changes assume that the state of rooms cannot change at runtime. 
        //Also this is bad


        //If NorthEdge
        if (EdgeDirection == EdgeDir.N)
        {
            SetTileType(0, 2, TileTypes.Solid);
        }
        //If NorthWestEdge
        else if (EdgeDirection == EdgeDir.NW)
        {
            SetTileType(0, 2, TileTypes.Solid);
            SetTileType(2, 0, TileTypes.Solid);
        }
        // If WestEdge
        else if (EdgeDirection == EdgeDir.W)
        {
            SetTileType(2, 0, TileTypes.Solid);
        }
        // If SouthWestEdge
        else if (EdgeDirection == EdgeDir.SW)
        {
            SetTileType(2, 0, TileTypes.Solid);
            SetTileType(4, 2, TileTypes.Solid);
        }
        // If SouthEdge
        else if (EdgeDirection == EdgeDir.S)
        {
            SetTileType(4, 2, TileTypes.Solid);
        }
        // If SouthEastEdge
        else if (EdgeDirection == EdgeDir.SE)
        {
            SetTileType(4, 2, TileTypes.Solid);
            SetTileType(2, 4, TileTypes.Solid);
        }
        // If EastEdge
        else if (EdgeDirection == EdgeDir.E)
        {
            SetTileType(2, 4, TileTypes.Solid);
        }
        // If NorthEastEdge
        else if (EdgeDirection == EdgeDir.NE)
        {
            SetTileType(2, 4, TileTypes.Solid);
            SetTileType(0, 2, TileTypes.Solid);
        }

    }
}
public enum EdgeDir { NOEDGE, N, NW, W, SW, S, SE, E, NE }