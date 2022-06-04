using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Inventory;
using TouhouPrideGameJam4.SO.Item;
using TouhouPrideGameJam4.UI;
using UnityEngine;
using static UnityEngine.UIElements.NavigationMoveEvent;

namespace TouhouPrideGameJam4.Character
{
    public class ACharacter : MonoBehaviour
    {
        /// <summary>
        /// Information about the character
        /// </summary>
        [SerializeField]
        protected SO.CharacterInfo _info;

        /// <summary>
        /// Items that the character has
        /// </summary>
        protected Dictionary<AItemInfo, int> _items = new();

        /// <summary>
        /// Current health of the character
        /// </summary>
        protected int _health;

        /// <summary>
        /// Equipped weapon
        /// </summary>
        protected WeaponInfo _equipedWeapon;

        private Animator _anim;

        // Used for smooth movement
        public Vector2 OldPos { set; get; }
        private float _moveTimer = 0f;

        protected readonly Dictionary<StatusType, int> _currentEffects = new();

        // Position
        private Vector2Int _position;
        public Vector2Int Position
        {
            set
            {
                _anim?.SetBool("IsWalking", true);
                _anim?.SetTrigger("StartWalking");
                OldPos = transform.position;
                _moveTimer = 0f;
                _position = value;
            }
            get
            {
                return _position;
            }
        }

        private Direction _direction;
        public Direction Direction
        {
            set
            {
                _direction = value;
                _anim?.SetInteger("Direction", (int)value);
            }
            get => _direction;
        }

        protected void Init()
        {
            _anim = GetComponent<Animator>();
            _health = _info.BaseHealth;
            _items = _info.StartingItems.ToDictionary(x => x, x => 1);
            _equipedWeapon = (WeaponInfo)_info.StartingItems.FirstOrDefault(x => x.Type == ItemType.Weapon);
            UpdateInventoryDisplay();
        }

        protected void UpdateC()
        {
            if (_moveTimer < 1f)
            {
                _moveTimer += Time.deltaTime * 2.5f;
                transform.position = Vector2.Lerp(OldPos, Position, Mathf.Clamp01(_moveTimer));
                if (_moveTimer >= 1f)
                {
                    _anim?.SetBool("IsWalking", false);
                }
            }
        }

        /// <summary>
        /// Called at the end of a turn
        /// </summary>
        public virtual void EndTurn()
        {
            for (int i = _currentEffects.Keys.Count - 1; i >= 0; i--)
            {
                var key = _currentEffects.Keys.ElementAt(i);
                if (_currentEffects[key] == 1)
                {
                    _currentEffects.Remove(key);
                }
                else
                {
                    _currentEffects[key]--;
                }
            }
        }

        public void AddStatus(StatusType status, int duration)
        {
            if (_currentEffects.ContainsKey(status))
            {
                _currentEffects[status] += duration;
            }
            else
            {
                _currentEffects.Add(status, duration);
            }
            UIManager.Instance.UpdateStatus(_currentEffects);
        }

        /// <summary>
        /// Update action bar and inventory display
        /// </summary>
        public virtual void UpdateInventoryDisplay()
        { }

        /// <summary>
        /// Remove an item from the character inventory
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void RemoveItem(AItemInfo item)
        {
            if (_items[item] == 1)
            {
                _items.Remove(item);
                // Our weapon was unequipped, we equip any other one we can
                if (item is WeaponInfo weapon && IsEquipped(weapon))
                {
                    Equip((WeaponInfo)_info.StartingItems.FirstOrDefault(x => x.Type == ItemType.Weapon));
                }
            }
            else
            {
                _items[item]--;
            }
            UpdateInventoryDisplay();
        }

        /// <summary>
        /// Is the character able to attack
        /// </summary>
        public bool CanAttack() => _equipedWeapon != null;

        /// <summary>
        /// Change the currently equipped weapon to the one given in parameter
        /// </summary>
        public void Equip(WeaponInfo weapon)
        {
            _equipedWeapon = weapon;
            UpdateInventoryDisplay();
        }

        /// <summary>
        /// Is the weapon given in parameter the one equipped
        /// </summary>
        public bool IsEquipped(WeaponInfo weapon) => _equipedWeapon == weapon;

        /// <summary>
        /// Show intentory
        /// </summary>
        /// <param name="inventory">Inentory script</param>
        /// <param name="baseFilter">Base filter to apply on items</param>
        public void ShowItems(InventoryUI inventory, ItemType? baseFilter)
        {
            var items = new Dictionary<AItemInfo, int>(_items);
            inventory.UpdateContent(this, items, baseFilter);
        }

        public virtual void TakeDamage(int amount)
        {
            if (amount > 0)
            {
                if (_currentEffects.ContainsKey(StatusType.Invicible))
                {
                    amount = 0;
                }
                else if (_currentEffects.ContainsKey(StatusType.BoostDefense))
                {
                    amount /= 2;
                }
            }

            _health -= amount;
            if (_health <= 0)
            {
                TurnManager.Instance.RemoveCharacter(this);
            }
            else if (_health > _info.BaseHealth)
            {
                _health = _info.BaseHealth;
            }

            // Display text with the damage done

            Color color;
            if (amount > 0) color = Color.red;
            else if (amount < 0) color = Color.green;
            else color = Color.yellow;
            TurnManager.Instance.SpawnDamageText(amount, color, Position.x + Random.Range(-.5f, .5f), Position.y + Random.Range(-.5f, .5f));
        }

        public void Attack(ACharacter target)
        {
            target.TakeDamage(_equipedWeapon.Damage * (_currentEffects.ContainsKey(StatusType.BoostAttack) ? 2 : 1));
        }

        public override string ToString()
        {
            return $"{name} - Health: {_health} / {_info.BaseHealth}";
        }
    }
}
