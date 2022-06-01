namespace TouhouPrideGameJam4.Map
{
    /// <summary>
    /// Instancied tile in the world
    /// </summary>
    public class Tile
    {
        public Tile(TileType type)
        {
            Type = type;
        }

        public TileType Type { private set; get; }
    }
}
