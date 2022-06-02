using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

namespace TouhouPrideGameJam4.Character
{
    public class ACharacter : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _info;

        private int _health;

        private Vector2Int _position;
        public Vector2Int Position
        {
            set
            {
                _position = value;
                transform.position = (Vector2)_position;
            }
            get
            {
                return _position;
            }
        }

        public void ShowItems(InventoryUI inventory)
        {
            inventory.UpdateContent(new[] { _info.DefaultWeapon });
        }

        protected void Init()
        {
            _health = _info.BaseHealth;
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                TurnManager.Instance.RemoveCharacter(this);
            }
        }

        public void Attack(ACharacter target)
        {
            target.TakeDamage(_info.DefaultWeapon.Damage);
        }

        public override string ToString()
        {
            return $"{name} - Health: {_health} / {_info.BaseHealth}";
        }
    }
}
