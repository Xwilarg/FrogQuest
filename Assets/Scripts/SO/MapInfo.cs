using TouhouPrideGameJam4.Map;
using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Header("Configuration")]
        public int MapSize;
        public int MaxEnemiesPerRoom;

        [Header("Rooms")]
        public TextAsset StartingRoom;
        public TextAsset[] Rooms;

        [Header("Parsing data")]
        public TileData[] ParsingData;
    }
}