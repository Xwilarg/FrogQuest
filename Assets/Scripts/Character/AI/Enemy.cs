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
                    x = Random.Range(Position.x - 5, Position.x + 6);
                    y = Random.Range(Position.y - 5, Position.y + 6);
                } while (!MapManager.Instance.IsTileWalkable(x, y) || TurnManager.Instance.GetCharacterPos(x, y) != null);
                Position = new(x, y);
            }
        }

        public int AttackCharge = 0;
    }
}
