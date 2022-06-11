using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Map;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Header("Configuration")]
        public int MapSize;
        public int MaxEnemiesPerRoom;
        public int IterationCount;

        [Header("Rooms")]
        public TextAsset StartingRoom;
        public TextAsset[] Rooms;

        [Header("Parsing data")]
        public TileData[] ParsingData;

        [Header("Enemies")]
        public DropRate<GameObject>[] EnemiesSpawn;

        [Header("Tiles")]
        public Sprite DoorSprite;
        public Sprite EntranceSprite, ExitDisabledSprite, ExitEnabledSprite;
    }
}