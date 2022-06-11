using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.UI;
using UnityEngine;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/Special/SpawnSpell", fileName = "SpawnSpell")]
    public class SpawnSpell : ConsumableInfo
    {
        public GameObject Prefab;
        public Team Team;

        public string SpawnedName;

        public override string Description => $"Summon a {SpawnedName}";

        public override void DoAction(ACharacter owner)
        {
            var pos = GetClosestEmptySpace(owner);
            owner.RemoveItem(this); // If GetClosestEmptySpace we make sure to not remove the object

            var c = Instantiate(Prefab, (Vector2)pos, Quaternion.identity).GetComponent<ACharacter>();
            c.Team = Team;
            c.Position = pos;

            TurnManager.Instance.AddCharacter(c);
        }

        public Vector2Int GetClosestEmptySpace(ACharacter owner)
        {
            var forward = owner.Position + owner.RelativeDirection;

            // We first try to look for the forward position
            if (MapManager.Instance.IsTileWalkable(forward.x, forward.y) && TurnManager.Instance.GetCharacterPos(forward.x, forward.y) == null)
            {
                return forward;
            }

            // If it doesn't work we look around us
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var pos = owner.Position + new Vector2Int(i, j);
                    if (MapManager.Instance.IsTileWalkable(pos.x, pos.y) && TurnManager.Instance.GetCharacterPos(pos.x, pos.y) == null)
                    {
                        return pos;
                    }
                }
            }

            throw new NoFreeSpaceException();
        }
    }
}