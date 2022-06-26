using UnityEngine;

namespace TouhouPrideGameJam4.Dialog.Parsing
{
    public record DialogStatement
    {
        public Sprite Image { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Color Color { set; get; }
    }
}