internal class TileBase
{
    public TileColor TileColor { get; private set; }
    public TileEffect TileEffect { get; private set; }

    public TileType TileType { get; private set; }

    public void SetTileType(TileType tileType)
    {
        TileType = tileType;
    }
}