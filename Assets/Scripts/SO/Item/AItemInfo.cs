using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    public abstract class AItemInfo : ScriptableObject
    {
        public abstract ItemType Type { get; }
        public abstract string Description { get; }

        public string Name;
    }
}