using System;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/ConsumableInfo", fileName = "ConsumableInfo")]
    public class ConsumableInfo : AItemInfo
    {
        public EffectType Effect;
        public int Value;

        public override ItemType Type => ItemType.Consummable;
        public override string Description => Effect switch
        {
            EffectType.Heal => $"Heal {Value} HP",
            EffectType.Invulnerability => $"Prevent you from taking damage for {Value} turns",
            EffectType.Spell => $"Use a spell card", // TODO
            _ => throw new System.NotImplementedException()
        };

        public override string ActionName => "Use";

        public override string ActionTooltip => "";

        public override void DoAction(ACharacter owner)
        {
            switch (Effect)
            {
                case EffectType.Heal:
                    owner.TakeDamage(-Value);
                    break;

                case EffectType.Invulnerability:
                    owner.AddStatus(StatusType.Invicible, Value);
                    break;

                default:
                    throw new NotImplementedException();
            }
            owner.RemoveItem(this);
        }
    }
}