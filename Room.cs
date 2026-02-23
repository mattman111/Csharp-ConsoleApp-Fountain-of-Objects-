using CSharpPlayersGuide.RichConsole;

partial class Room
{
    // All rooms are 5x5 tiles
    private const int _xlength = 5;
    private const int _ylength = 5;

    //Explicit backing fields
    private EdgeDir _edgeDiection;
    private RoomType _roomType;
    private RoomStatus _roomStatus;
    private bool _playerPresent;

    //Number of tiles in a row
    public const int NumOfRoomTilesInRow = _xlength;

    public Tile[,] TilesData { get; private set; }

    public EdgeDir EdgeDirection
    {
        get => _edgeDiection;
        set { _edgeDiection = value; ApplyEdgeConstraints(); }
    }

    public RoomType RoomType
    {
        get => _roomType;
        set { _roomType = value; RefreshRoomData(); }
    }
    public RoomStatus RoomStatus
    {
        get => _roomStatus; 
        set { _roomStatus = value; RefreshRoomData(); }
    }

    public bool PlayerPresent
    {
        get => _playerPresent;
        set { _playerPresent = value; RefreshRoomData(); }
    }

    public int RoomInGridX { get; init; }
    public int RoomInGridY { get; init; }

    public Room(int roomGridX, int roomGridY)
    {
        TilesData = new Tile[_xlength, _ylength];
        EdgeDirection = EdgeDir.NOEDGE;
        RoomType = RoomType.Empty;
        RoomStatus = RoomStatus.Unknown;
        RoomInGridX = roomGridX;
        RoomInGridY = roomGridY;
        PlayerPresent = false;
        RefreshRoomData();
    }
    /// <summary>
    /// Whenever Type, Status, or Edge changes, this method rebuild TilesData for the affected room.
    /// </summary>
    public void RefreshRoomData()
    {
        TileColor color = GetCurrentRoomColor();
        for (int i = 0; i < _xlength; i++)
        {
            for (int j = 0; j < _ylength; j++)
            {
                TilesData[i, j] = GenerateTileData(i, j);
            }
        }
    }

    //Chonky method that idk how to make smaller
    private Tile GenerateTileData(int i, int j)
    {
        // 1. Handle Walls/Edges
        if (IsWallCoordinate(i, j)) return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        // 2. Special Room Centers
        if (i == 2 && j == 2)
        {
            return RoomType switch
            {
                RoomType.Fountain => new Tile(TileType.Fountain, TileColor.Aqua, TileEffect.Blink),
                RoomType.Pit => new Tile(TileType.Pit, TileColor.LightGrey, TileEffect.Blink),
                RoomType.Maelstrom => new Tile(TileType.Maelstrom, TileColor.Yellow, TileEffect.Blink),
                RoomType.Amarok => new Tile(TileType.Amarok, TileColor.Red, TileEffect.Blink),
                _ => new Tile(TileType.Empty, TileColor.Black, TileEffect.None)
            };
        }

        // 3. Entrance Doorway(s) Logic
        if (RoomType == RoomType.Entrance && IsEntrace(i,j))
        {
            return new Tile(TileType.Entrance, TileColor.Orange, TileEffect.None);
        }

        // 4. Pit Doorway(s) Logic
        if (RoomType == RoomType.Pit && IsEntrace(i, j))
        {
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);
        }
        // 5. Maelstrom Doorway(s) Logic
        if (RoomType == RoomType.Maelstrom && IsEntrace(i, j))
        {
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);
        }
        // 6. Amark Doorway(s) Logic
        if (RoomType == RoomType.Amarok && IsEntrace(i, j))
        {
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);
        }

        return new Tile(TileType.Empty, TileColor.Black, TileEffect.None);
    }

    private bool IsWallCoordinate(int i, int j)
    {
        // All possible cordinates leaving [0,2], [2,0], [2,4], [4,2] as potential doorsways.
        bool isCorner = (i == 0 || i == 4) && (j == 0 || j == 1 || j == 3 || j == 4);
        bool isSideMid = (i == 1 || i == 3) && (j == 0 || j == 4);
        return isCorner || isSideMid;
    }

    private bool IsDoorwayCoordinate(int i, int j)
    {
        bool isDoorway = (i == 0 || i == 2 || i == 4) && (j == 0 || j == 2 || j == 4);
        return isDoorway;
    }

    private bool IsEntrace(int i, int j)
    {
        if (IsDoorwayCoordinate(i,j))
        {
            if (EdgeDirection.ToString().Contains("N") && (i == 0 && j == 2))
            {
                return true;
            }
            if (EdgeDirection.ToString().Contains("S") && (i == 4 && j == 2))
            {
                return true;
            }
            if (EdgeDirection.ToString().Contains("E") && (i == 2 && j == 4))
            {
                return true;
            }
            if (EdgeDirection.ToString().Contains("W") && (i == 2 && j == 0))
            {
                return true;
            }
        }
        return false;
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

        switch (TilesData[x, y].TileColor)
        {
            case TileColor.LightGrey:
                return Colors.LightGray;
            case TileColor.Grey:
                return Colors.Gray;
            case TileColor.DarkGrey:
                return Colors.DimGray;
            case TileColor.Blue:
                return Colors.Blue;
            case TileColor.White:
                return Colors.White;
            case TileColor.Orange:
                return Colors.Orange;
            case TileColor.Aqua:
                return Colors.LightCyan;
            case TileColor.Yellow:
                return Colors.Yellow;
            default:
                return Colors.Red;
        }
    }

    public TileColor GetCurrentRoomColor()
    {
        if (RoomStatus == RoomStatus.Unknown)
        {
            return TileColor.Black;
        }

        switch (_roomType)
        {
            case RoomType.Empty:
                return TileColor.DarkGrey;
            case RoomType.Entrance:
                return TileColor.Yellow;
            default:
                return TileColor.Red;
        }

    }

    public void SetTile(int x, int y, TileType tileType, TileColor color, TileEffect effect)
    {
        TilesData[x, y] = new Tile(tileType, color, effect);
        // Update Data()
    }

    public void SetRoomType(RoomType roomtype)
    {
        RoomType = roomtype;
        //UPDATE DATA()
    }

    public string GetTileArt(int x, int y)
    {
        switch (TilesData[x, y].TileType)
        {
            case TileType.Empty:
                return "  ";
            case TileType.Solid:
                return "🧱";
            case TileType.Entrance:
                return "🍂";
            case TileType.Player:
                return "😐";
            case TileType.Fountain:
                return "⛲";
            case TileType.Pit:
                return "🕳️";
            case TileType.Maelstrom:
                return "🌪️";
            case TileType.Amarok:
                return "🐺";
            default:
                //If we got here something went horribly wrong.
                return "?";
        }
    }



    public TileColor ReturnRoomColor()
    {
        switch (RoomStatus)
        {
            case RoomStatus.Unknown:
                return TileColor.DarkGrey;
            case RoomStatus.Known:
                switch (RoomType)
                {
                    case RoomType.Entrance:
                        return TileColor.White;
                    case RoomType.Empty:
                        return TileColor.DarkGrey;
                }
                return TileColor.Blue;
            default:
                return TileColor.Blue;
        }
    }

    private void ApplyEdgeConstraints()
    {
        // Close open doorways if the EdgeDirection says there is a boundary there
        if (EdgeDirection.ToString().Contains("N")) TilesData[0, 2] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("S")) TilesData[4, 2] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("W")) TilesData[2, 0] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("E")) TilesData[2, 4] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
    }
}
//C IS FOR CENTER 
public enum EdgeDir { NOEDGE, N, NW, W, SW, S, SE, E, NE }