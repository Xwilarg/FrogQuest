using System.Collections.Generic;
using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/ShopInfo", fileName = "ShopInfo")]
    public class ShopInfo : ScriptableObject
    {
        public BuyableItem[] Items
        {
            get
            {
                List<BuyableItem> items = new();
                items.AddRange(Weapons);
                items.AddRange(Spells);
                items.AddRange(Potions);
                return items.ToArray();
            }
        }


        public BuyableItem[] Weapons, Spells, Potions;
    }
}