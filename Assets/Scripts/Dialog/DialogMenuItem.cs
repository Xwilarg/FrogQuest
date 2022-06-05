using UnityEngine.UI;
using UnityEngine;
using System;
namespace TouhouPrideGameJam4.VisualNovel
{
    /// <summary>
    /// an item in a menu in the dialog
    /// </summary>
    [Serializable]
    public record DialogMenuItem
    {
        /// <summary> The next dialog label to jump to </summary> 
        public string Label { get; set; }
        /// <summary> The game flag to set if this option is chosen </summary>
        public string Flag { get; set; }
        /// <summary> A line of text for the menu </summary>
        public string Text { get; set; }
    }
}