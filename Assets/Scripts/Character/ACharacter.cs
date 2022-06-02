using System.Collections.Generic;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Character
{
    public class ACharacter : MonoBehaviour
    {
        [SerializeField]
        private SO.CharacterInfo _info;

        private List<AItemInfo> _items = new();

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
            var items = new List<AItemInfo>(_info.StartingItems)
            {
                _info.DefaultWeapon
            };
            inventory.UpdateContent(this, items.ToArray(), null);
        }

        protected void Init()
        {
            _health = _info.BaseHealth;
            _items.AddRange(_info.StartingItems);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                TurnManager.Instance.RemoveCharacter(this);
            }
            else if (_health > _info.BaseHealth)
            {
                _health = _info.BaseHealth;
            }
            TurnManager.Instance.UpdateDebugText();
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
