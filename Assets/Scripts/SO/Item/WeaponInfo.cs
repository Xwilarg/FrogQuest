using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/WeaponInfo", fileName = "WeaponInfo")]
    public class WeaponInfo : AItemInfo
    {
        public int Damage;

        public override ItemType Type => ItemType.Weapon;
        public WeaponEffectInfo[] HitEffects;

        public int Range = 1;

        public bool IsHeal;

        public override string Description => $"{Damage} damage";

        public override string ActionName => "Equip";

        public override string ActionTooltip => "Set the item as your main weapon";

        public override void DoAction(ACharacter owner)
        {
            owner.Equip(this);
        }
    }
}