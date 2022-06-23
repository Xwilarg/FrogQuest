using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Character.AI
{
    public class Enemy : ACharacter
    {
        private void Start()
        {
            Init(Team);
        }

        private void Update()
        {
            UpdateC();
        }

        public override void TakeDamage(WeaponInfo weapon, int amount)
        {
            base.TakeDamage(weapon, amount);

            if (IsBoss) // Boss teleport on hit
            {
                int x, y;
                do
                {
                    x = PlayerController.Instance.Position.x;
                    y = PlayerController.Instance.Position.y;
                    if (Random.Range(0, 2) == 0)
                    {
                        x = Random.Range(x - 5, x + 6);
                    }
                    else
                    {
                        y = Random.Range(y - 5, y + 6);
                    }
                } while (Vector2Int.Distance(new(x, y), PlayerController.Instance.Position) <= 2 || !MapManager.Instance.IsTileWalkable(x, y) || TurnManager.Instance.GetCharacterPos(x, y) != null);
                Position = new(x, y);
            }
        }

        public int AttackCharge = 0;
    }
}
