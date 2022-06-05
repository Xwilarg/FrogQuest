using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CharacterInfo", fileName = "CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public int BaseHealth;

        public AItemInfo[] StartingItems;

        public WeaponInfo DefaultWeapon;

        public bool DoesDisappearAfterAttacking;
    }
}