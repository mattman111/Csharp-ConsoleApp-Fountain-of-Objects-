using CSharpPlayersGuide.RichConsole;

partial class Room
{
    // All rooms are 5x5 tiles
    private const int _xlength = 5;
    private const int _ylength = 5;

    //Number of tiles in a row
    public const int NumOfRoomTilesInRow = _xlength;

    public Tile[,] TilesData { get; private set;  }

    public EdgeDir EdgeDirection { get; private set; }

    public RoomType RoomType { get; private set; }
    public RoomStatus RoomStatus { get; private set; }
    public RoomEvents Events { get; private set; }

    public int RoomGridX { get; init; }
    public int RoomGridY { get; init; }

    public Room(int roomGridX, int roomGridY)
    {
        TilesData = new Tile[_xlength, _ylength];
        EdgeDirection = EdgeDir.NOEDGE;
        RoomType = RoomType.EventRoom;
        RoomStatus = RoomStatus.Unknown;
        Events = RoomEvents.None;
        RoomGridX = roomGridX;
        RoomGridY = roomGridY;
        InitializeRoom();
    }

    public void InitializeRoom()
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

                    TilesData[i, j] = new Tile(TileTypes.Solid, TileColor.DarkGrey, TileEffect.None);

                }

                else

                {

                    TilesData[i, j] = new Tile(TileTypes.Empty, TileColor.Black, TileEffect.None);

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
            default:
                return Colors.Red;
        }
    }

    public void SetTile(int x, int y, TileTypes tileType,  TileColor color, TileEffect effect)
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
        switch (RoomStatus)
        {
            case RoomStatus.Unknown:
                return TileColor.DarkGrey;
            case RoomStatus.Known:
                switch (RoomType)
                {
                    case RoomType.Entrance:
                        return TileColor.White;
                    case RoomType.EventRoom:
                        return TileColor.DarkGrey;
                }
                return TileColor.Blue;
            default:
                return TileColor.Blue;
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
        ApplyEdgeConstraints();

    }
private void ApplyEdgeConstraints()
    {
        // Close the doors if the EdgeDirection says there is a boundary there
        if (EdgeDirection.ToString().Contains("N")) TilesData[0, 2] = new Tile(TileTypes.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("S")) TilesData[4, 2] = new Tile(TileTypes.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("W")) TilesData[2, 0] = new Tile(TileTypes.Solid, ReturnRoomColor(), TileEffect.None);
        if (EdgeDirection.ToString().Contains("E")) TilesData[2, 4] = new Tile(TileTypes.Solid, ReturnRoomColor(), TileEffect.None);
    }
}
public enum EdgeDir { NOEDGE, N, NW, W, SW, S, SE, E, NE }
