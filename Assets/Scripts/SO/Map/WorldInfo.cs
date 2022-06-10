using UnityEngine;

namespace TouhouPrideGameJam4.SO.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/WorldInfo", fileName = "WorldInfo")]
    public class WorldInfo : ScriptableObject
    {
        public string Name;
        public MapInfo MapInfo;
        public int StageCount;
    }
}