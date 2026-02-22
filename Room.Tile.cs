partial class Room
{
    public struct Tile
    {
        public Tile(TileTypes tileType, TileColor tileColor, TileEffect tileEffect)
        {
            TileType = tileType;
            TileColor = tileColor;
            TileEffect = tileEffect;
        }

        public TileTypes TileType { get; private set; }
        public TileColor TileColor { get; private set; }
        public TileEffect TileEffect { get; private set; }

        public void SetTileType(TileTypes tileType)
        {
            TileType = tileType;
        }

    }
}
