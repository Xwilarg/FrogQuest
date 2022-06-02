using System;
using UnityEngine;
/// <summary>
/// Information on a tile
/// </summary>
namespace TouhouPrideGameJam4.Map
{
    [Serializable]
    public class TileData
    {
        /// <summary>
        /// Character that we are looking for in the file
        /// </summary>
        public char Character;

        /// <summary>
        /// Associated tile
        /// </summary>
        public TileType Type;

        /// <summary>
        /// Color used for gizmo debugging
        /// </summary>
        public Color GizmoColor;

        /// <summary>
        /// Is the player allowed to walk on this
        /// </summary>
        public bool CanBeWalkedOn;
    }
}
