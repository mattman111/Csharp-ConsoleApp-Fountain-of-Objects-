class Obstacle
{
    public TileType ObstacleType { get; private set; } 
    
    public int TileX { get; private set;  }
    public int TileY { get; private set; }

    public Obstacle(RoomType obstacleType, int tileX, int tileY)
    {
        switch(obstacleType)
        {
            case RoomType.Fountain:
                ObstacleType = TileType.Fountain;
                break;
            case RoomType.Pit:
                ObstacleType = TileType.Pit;
                break;
            case RoomType.Maelstrom:
                ObstacleType = TileType.Maelstrom;
                break;
            case RoomType.Amarok:
                ObstacleType = TileType.Amarok;
                break;
        }

        TileX = tileX;
        TileY = tileY;
    }
}
