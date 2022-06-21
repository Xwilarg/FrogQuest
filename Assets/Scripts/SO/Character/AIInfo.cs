using UnityEngine;

namespace TouhouPrideGameJam4.SO.Character
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character/AIInfo", fileName = "AIInfo")]
    public class AIInfo : ScriptableObject
    {
        [Tooltip("If the enemy is further than this distance from the target, it doesn't go towards him")]
        public int MaxDistanceToMove;
    }
}