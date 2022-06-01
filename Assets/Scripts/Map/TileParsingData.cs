using System;
using UnityEngine;
/// <summary>
/// Information on how a tile should be parsed
/// </summary>
namespace TouhouPrideGameJam4.Map
{
    [Serializable]
    public class TileParsingData
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
    }
}
