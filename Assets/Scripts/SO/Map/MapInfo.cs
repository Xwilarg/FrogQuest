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
        [Range(0f, 1f)]
        public float ChestPerRoom;
        public int IterationCount;
        public bool IsBossRoom;

        [Header("Rooms")]
        public TextAsset StartingRoom;
        public TextAsset[] Rooms;

        [Header("Parsing data")]
        public TileData[] ParsingData;

        [Header("Enemies")]
        public DropRate<GameObject>[] EnemiesSpawn;

        [Header("Tiles")]
        public Sprite DoorSprite;
        public Sprite ChestSprite;
        public Sprite EntranceSprite, ExitDisabledSprite, ExitEnabledSprite;

        [Header("Metadata")]
        public string Name;
        public int StageCount;
        public Sprite Image;

        [Header("Audio")]
        public AudioClip IntroSong;
        public AudioClip MainSong;
    }
}