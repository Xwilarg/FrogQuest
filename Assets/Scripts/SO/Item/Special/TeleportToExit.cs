using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using TouhouPrideGameJam4.Map;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/Special/TeleportToExit", fileName = "TeleportToExit")]
    public class TeleportToExit : ConsumableInfo
    {
        public override string Description => $"Teleport to the exit";

        public override void DoAction(ACharacter owner)
        {
            var r = MapManager.Instance.Exit;
            if (r == default)
            {
                return;
            }
            var (X, Y) = r;
            MapManager.Instance.DiscoverRoom(X, Y);
            owner.Position = new(X, Y);
            TurnManager.Instance.TryEnableGoal();
        }

        public override ItemType Type => ItemType.Spell;
    }
}