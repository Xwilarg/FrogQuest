using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/WeaponInfo", fileName = "WeaponInfo")]
    public class WeaponInfo : AItemInfo
    {
        public int Damage;

        public override ItemType Type => ItemType.Weapon;
        public override string Description => $"{Damage} damages";
    }
}