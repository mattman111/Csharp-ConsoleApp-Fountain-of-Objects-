using CSharpPlayersGuide.RichConsole;
using System.Numerics;
partial class Room
{
    // All rooms are 5x5 tiles
    private const int _xlength = 5;
    private const int _ylength = 5;

    private readonly Dictionary<Vector2, Entity> _entities = new();

    //Explicit backing fields
    private EdgeDir _edgeDiection;
    private RoomType _roomType;
    private RoomStatus _roomStatus;
    private bool _playerPresent;

    //The default starting position (off board)
    private Vector2 _playerPosition = new Vector2(-1,-1);

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

    public Vector2 PlayerPosition
    {
        get => _playerPosition;
        set { _playerPosition = value; RefreshRoomData(); }
    }

    public int RoomInGridX { get; init; }
    public int RoomInGridY { get; init; }

    public Room(int roomGridX, int roomGridY)
    {
        TilesData = new Tile[_xlength, _ylength];
        EdgeDirection = EdgeDir.None;
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
        for (int i = 0; i < _xlength; i++)
        {
            for (int j = 0; j < _ylength; j++)
            {
                TilesData[i, j] = GenerateTileData(i, j);
            }
        }
    }

    public void AddEntity(int x, int y, Entity entity)
    {
        var pos = new Vector2(x, y);
        _entities[pos] = entity;
        RefreshRoomData();
    }

    public void RemoveEntity(int x, int y, Entity entity)
    {
        var pos = new Vector2(x, y);
        _entities.Remove(pos);
        RefreshRoomData();
    }

    private Tile GenerateTileData(int i, int j)
    {
        Vector2 currentPos = new Vector2(i, j);

        // PRIORITY OF LOGIC HANDLING
        // 1. PLAYER
        if (_playerPresent && _playerPosition == currentPos)
            return new Tile(TileType.Player, TileColor.White, TileEffect.None);

        // 2. Entities (Pits, Amarks, Maelstroms, Fountains, etc.)
        if (_entities.TryGetValue(currentPos, out var entity))
            return new Tile(entity.Type, entity.Color, entity.Effect);

        // 3. Environment (Entrance then Walls)
        if (IsEdge(i, j) && _roomType == RoomType.Entrance)
            return new Tile(TileType.Entrance, TileColor.Orange, TileEffect.None);

        if (IsWallCoordinate(i, j))
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        if (IsEdge(i, j) && _roomType == RoomType.Pit)
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        if (IsEdge(i, j) && _roomType == RoomType.Maelstrom)
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        if (IsEdge(i, j) && _roomType == RoomType.Amarok)
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        if (IsEdge(i, j) && _roomType == RoomType.Empty)
            return new Tile(TileType.Solid, TileColor.DarkGrey, TileEffect.None);

        //There is nothing to draw
        return new Tile(TileType.Empty, TileColor.Black, TileEffect.None);
    }

    private bool IsWallCoordinate(int i, int j)
    {
        // If it's on the edge but NOT one of the 4 middle-points (the doors)
        bool isEdge = (i == 0 || i == 4 || j == 0 || j == 4);
        return isEdge && !IsDoorwayCoordinate(i, j);
    }

    private bool IsDoorwayCoordinate(int i, int j)
    {
        // The specific middle spots of each wall
        return (i == 0 && j == 2) || (i == 4 && j == 2) ||
               (i == 2 && j == 0) || (i == 2 && j == 4);
    }

    private bool IsEdge(int i, int j)
    {
        if (IsDoorwayCoordinate(i,j))
        {
            if (EdgeDirection.HasFlag(EdgeDir.N) && (i == 0 && j == 2))
            {
                return true;
            }
            if (EdgeDirection.HasFlag(EdgeDir.S) && (i == 4 && j == 2))
            {
                return true;
            }
            if (EdgeDirection.HasFlag(EdgeDir.E) && (i == 2 && j == 4))
            {
                return true;
            }
            if (EdgeDirection.HasFlag(EdgeDir.W) && (i == 2 && j == 0))
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
        if (RoomStatus == RoomStatus.Unknown)
        {
            return Colors.Black;
        }

        switch (TilesData[x, y].TileColor)
        {
            case TileColor.LightGray:
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

    public void SetRoomType(RoomType roomtype)
    {
        RoomType = roomtype;
        //UPDATE DATA()
    }

    public string GetTileArt(int x, int y)
    {
        if (RoomStatus == RoomStatus.Unknown)
        {
            return "  ";
        }

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
        if (EdgeDirection.HasFlag(EdgeDir.N)) TilesData[0, 2] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.HasFlag(EdgeDir.S)) TilesData[4, 2] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.HasFlag(EdgeDir.W)) TilesData[2, 0] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.HasFlag(EdgeDir.E)) TilesData[2, 4] = new Tile(TileType.Solid, ReturnRoomColor(), TileEffect.None);
    }
}

//EdgeDir needs to be a bit field or everything is harder. 
[Flags]
public enum EdgeDir { None = 0, N = 1, S = 2, E = 4, W = 8, NE = N | E, NW = N | W, SE = S | E, SW = S | W }