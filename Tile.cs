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

