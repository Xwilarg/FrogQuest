using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/ShopInfo", fileName = "ShopInfo")]
    public class ShopInfo : ScriptableObject
    {
        public BuyableItem[] Items;
    }
}