using TouhouPrideGameJam4.Character;
using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/StatusInfo", fileName = "StatusInfo")]
    public class StatusInfo : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;
        public StatusType Effect;
        public bool IsNegative;
    }
}