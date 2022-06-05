using UnityEngine.UI;
using UnityEngine;
using System;
namespace TouhouPrideGameJam4.VisualNovel
{
    [Serializable]
    public record DialogStatement
    {
        public string ImageName { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}