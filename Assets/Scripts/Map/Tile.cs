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
            SpriteRendererMain = sr;
        }

        public TileType Type { set; get; }
        public TileContentType Content { set; get; }
        public SpriteRenderer SpriteRendererMain { set; get; }
        public SpriteRenderer SpriteRendererSub { set; get; }
    }
}
