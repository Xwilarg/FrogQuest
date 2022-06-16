using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Character.Player;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Character
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character/FollowerInfo", fileName = "FollowerInfo")]
    public class FollowerInfo : ScriptableObject
    {
        public FollowerType Type;
        public StatusType Status;
    }
}