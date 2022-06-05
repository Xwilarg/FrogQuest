using UnityEngine.UI;
using UnityEngine;
using System;
namespace TouhouPrideGameJam4.VisualNovel
{
    /// <summary> 
    /// A DialogStatement which presents a player with several choices.
    /// Each choice may set a flag, and jump to a new label
    /// </summary>
    [Serializable]
    public record DialogMenu : DialogStatement
    { 
        /// <summary> the list of menu items the player may select </summary>
        public DialogMenuItem[] MenuItems { get; set; }
    }
}