using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Map
{
    /// <summary>
    /// Instantiated tile in the world
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

        public AItemInfo ItemDropped;
        public SpriteRenderer SpriteRendererMain { set; get; }
        public SpriteRenderer SpriteRendererSub { set; get; }
        public SpriteRenderer SpriteRendererItem { set; get; }
    }
}
