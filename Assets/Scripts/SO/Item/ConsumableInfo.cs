using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/ConsumableInfo", fileName = "ConsumableInfo")]
    public class ConsumableInfo : AItemInfo
    {
        public EffectType Effect;
        public int Value;

        public override ItemType Type => ItemType.Consummable;
        public override string Description => Effect switch
        {
            EffectType.Heal => $"Heal {Value} HP",
            _ => throw new System.NotImplementedException()
        };
    }
}