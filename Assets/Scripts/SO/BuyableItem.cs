using System;
using TouhouPrideGameJam4.SO.Item;

namespace TouhouPrideGameJam4.SO
{
    [Serializable]
    public class BuyableItem
    {
        public AItemInfo Item;
        public int Price;
    }
}