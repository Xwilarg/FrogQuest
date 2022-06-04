using UnityEngine;

namespace TouhouPrideGameJam4.Map
{
    /// <summary>
    /// Instancied tile in the world
    /// </summary>
    public class Tile
    {
        public Tile(TileType type, SpriteRenderer sr)
        {
            Type = type;
            SpriteRenderer = sr;
        }

        public TileType Type { set; get; }
        public SpriteRenderer SpriteRenderer { set; get; }
    }
}
