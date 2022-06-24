using System;
using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/ConsumableInfo", fileName = "ConsumableInfo")]
    public class ConsumableInfo : AItemInfo
    {
        public EffectType[] Effect;
        public int Value;

        public override string Description
        {
            get
            {
                return string.Join("\n", Effect.Select(x =>
                    x switch
                    {
                        EffectType.InfiniteItems => $"Infinite items for {Value} turns",
                        EffectType.Heal => $"Heal {Value} HP",
                        EffectType.Invulnerability => $"Make you invulnerable for {Value} turns",
                        EffectType.BoostAttack => $"Double your attack for {Value} turns",
                        EffectType.BoostDefense => $"Half incoming damage for {Value} turns",
                        _ => throw new NotImplementedException()
                    })
                );
            }
        }

        public override string ActionName => "Use";

        public override string ActionTooltip => "Consume the current item";

        public override ItemType Type => ItemType.Potion;

        public override void DoAction(ACharacter owner)
        {
            foreach (var e in Effect)
            {
                switch (e)
                {
                    case EffectType.InfiniteItems:
                        owner.hasInfiniteItems = true;
                        break;
                    case EffectType.Heal:
                        owner.TakeDamage(null, -Value);
                        break;

                    case EffectType.Invulnerability:
                        owner.AddStatus(StatusType.Invincible, Value);
                        break;

                    case EffectType.BoostAttack:
                        owner.AddStatus(StatusType.AttackBoosted, Value);
                        break;

                    case EffectType.BoostDefense:
                        owner.AddStatus(StatusType.DefenseBoosted, Value);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            if(!owner.hasInfiniteItems){
                owner.RemoveItem(this);
            }
        }
    }
}