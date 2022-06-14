using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Character
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character/CharacterInfo", fileName = "CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public int BaseHealth;

        public DropRate<AItemInfo>[] StartingItems;

        public WeaponInfo DefaultWeapon;

        public bool DoesDisappearAfterAttacking;
    }
}