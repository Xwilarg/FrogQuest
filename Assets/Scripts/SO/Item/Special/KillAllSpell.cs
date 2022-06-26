using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/Special/KillAllSpell", fileName = "KillAllSpell")]
    public class KillAllSpell : ConsumableInfo
    {
        public override string Description => $"Kill all enemies";

        public override void DoAction(ACharacter owner)
        {
            var e = TurnManager.Instance.Enemies;
            while (e.Any())
            {
                TurnManager.Instance.RemoveCharacter(e.First());
            }
        }

        public override ItemType Type => ItemType.Spell;
    }
}