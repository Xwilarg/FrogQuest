using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    public abstract class AItemInfo : ScriptableObject
    {
        /// <summary>
        /// What kind of object it is
        /// </summary>
        public abstract ItemType Type { get; }
        /// <summary>
        /// Description explaining to the player what the item does
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Description explaining what the actual item is
        /// </summary>
        [TextArea]
        public string UtilityDescription;

        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name;
        /// <summary>
        /// Sprite to show in the action bar
        /// </summary>
        public Sprite Sprite;

        // When trying to use the item

        /// <summary>
        /// Callback when the item is used
        /// </summary>
        /// <param name="owner">Owner of the item</param>
        public abstract void DoAction(ACharacter owner);
        /// <summary>
        /// Name displayed to show what kind of action it is
        /// </summary>
        public abstract string ActionName { get; }
        /// <summary>
        /// Tooltip explaning what the action will do
        /// </summary>
        public abstract string ActionTooltip { get; }

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