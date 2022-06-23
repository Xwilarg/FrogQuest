using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/Special/RemedySpell", fileName = "RemedySpell")]
    public class RemedySpell : ConsumableInfo
    {
        public StatusType[] Status;
        public int TurnCount;

        public override string Description => $"Remove all negative status and grants {string.Join(", ", Status)} for {TurnCount} turns";

        public override void DoAction(ACharacter owner)
        {
            owner.RemoveItem(this);
            var effects = owner.CurrentEffects;
            for (int i = effects.Length - 1; i >= 0; i--)
            {
                var status = GameManager.Instance.GetStatusFromType(effects[i]);
                if (status.IsNegative)
                {
                    owner.RemoveStatus(status.Effect);
                }
            }
            foreach (var s in Status)
            {
                owner.AddStatus(s, TurnCount);
            }
        }

        public override ItemType Type => ItemType.Spell;
    }
}