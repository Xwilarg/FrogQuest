using TouhouPrideGameJam4.Character;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Character
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character/FollowerInfo", fileName = "FollowerInfo")]
    public class FollowerInfo : ScriptableObject
    {
        public StatusType Status;
        public Sprite Sprite;
    }
}