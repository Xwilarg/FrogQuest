using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    public abstract class AItemInfo : ScriptableObject
    {
        public abstract ItemType Type { get; }
        public abstract string Description { get; }

        [TextArea]
        public string UtilityDescription;

        public string Name;
        public Sprite Sprite;

        public static bool operator ==(AItemInfo a, AItemInfo b)
        {
            if (a is null)
            {
                return b is null;
            }
            if (b is null)
            {
                return false;
            }
            return a.Name == b.Name;
        }

        public static bool operator !=(AItemInfo a, AItemInfo b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            if (other is not AItemInfo item)
            {
                return false;
            }
            return item == this;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}