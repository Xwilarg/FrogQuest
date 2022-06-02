using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<AItemInfo, int> _items = new();

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

        public void RemoveItem(AItemInfo item)
        {
            if (_items[item] == 1)
            {
                _items.Remove(item);
            }
            else
            {
                _items[item]--;
            }
        }

        public void ShowItems(InventoryUI inventory, ItemType? baseFilter)
        {
            var items = new Dictionary<AItemInfo, int>(_items)
            {
                { _info.DefaultWeapon, 1 }
            };
            inventory.UpdateContent(this, items, baseFilter);
        }

        protected void Init()
        {
            _health = _info.BaseHealth;
            _items = _info.StartingItems.ToDictionary(x => x, x => 1);
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
