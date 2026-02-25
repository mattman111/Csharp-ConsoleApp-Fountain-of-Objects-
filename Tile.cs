class Tile
{

    public Tile(TileType tileType, TileColor tileColor, TileEffect tileEffect)
    {
        TileType = tileType;
        TileColor = tileColor;
        TileEffect = tileEffect;
    }

    public TileType TileType { get; private set; }
    public TileColor TileColor { get; private set; }
    public TileEffect TileEffect { get; private set; }

    public void SetTileType(TileType tileType)
    {
        TileType = tileType;
    }

}

//All possible colors of tiles
public enum TileColor { Black, Grey, Orange, Yellow, White, DarkGrey, LightGray, Blue, Red, Aqua }

//All possible effects of tiles
public enum TileEffect { None, Blink, DoubleUnderline }

//All possible types of tiles
public enum TileType { Empty, Solid, Player, Entrance, Fountain, Pit, Maelstrom, Amarok }