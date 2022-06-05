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
            EffectType.Invulnerability => $"Make you invulnerable for {Value} turns",
            EffectType.BoostAttack => $"Double your attack for {Value} turns",
            EffectType.BoostDefense => $"Half incoming damage for {Value} turns",
            _ => throw new NotImplementedException()
        };

        public override string ActionName => "Use";

        public override string ActionTooltip => "Consume the current item";

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

                case EffectType.BoostAttack:
                    owner.AddStatus(StatusType.BoostAttack, Value);
                    break;

                case EffectType.BoostDefense:
                    owner.AddStatus(StatusType.BoostDefense, Value);
                    break;

                default:
                    throw new NotImplementedException();
            }
            owner.RemoveItem(this);
        }
    }
}