using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouhouPrideGameJam4.Character.Player
{
    public class PlayerController : ACharacter
    {
        public static PlayerController Instance { get; private set; }

        [SerializeField]
        private AudioClip _stepSound;

        private AudioSource _source;

        private void Awake()
        {
            Instance = this;
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            UpdateC();
        }

        public override void EndTurn()
        {
            base.EndTurn();
            UIManager.Instance.UpdateStatus(_currentEffects);
        }

        public override void UpdateInventoryDisplay()
        {
            UIManager.Instance.ShortcutEquipped.sprite = _equipedWeapon.Sprite;
            UIManager.Instance.ShortcutEquipped.color = UIManager.Instance.ShortcutEquipped.sprite == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
            int index = 0;
            foreach (var btn in UIManager.Instance.ShortcutInventory)
            {
                btn.Clear();
            }
            foreach (var item in _items)
            {
                if (item.Key == _equipedWeapon)
                {
                    continue;
                }

                UIManager.Instance.ShortcutInventory[index].SetContent(item.Key);
                index++;
                if (index == UIManager.Instance.ShortcutInventory.Length)
                {
                    break;
                }
            }
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);

            UIManager.Instance.SetHealth(_health / (float)_info.BaseHealth);
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                var mov = value.ReadValue<Vector2>();
                if (mov.x != 0f || mov.y != 0f)
                {
                    if (Mathf.Abs(mov.x) > Mathf.Abs(mov.y))
                    {
                        if (TurnManager.Instance.MovePlayer(mov.x > 0 ? 1 : -1, 0))
                        {
                            _source.PlayOneShot(_stepSound);
                        }
                    }
                    else
                    {
                        if (TurnManager.Instance.MovePlayer(0, mov.y > 0 ? 1 : -1))
                        {
                            _source.PlayOneShot(_stepSound);
                        }
                    }
                }
            }
        }

        public void OnInventory(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                TurnManager.Instance.ToggleInventory();
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                UIManager.Instance.UseCurrent();
            }
        }

        public void OnWait(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                TurnManager.Instance.PlayEnemyTurn();
            }
        }

        public void OnShortcut1(InputAction.CallbackContext value) => OnShortcut(value, 0);
        public void OnShortcut2(InputAction.CallbackContext value) => OnShortcut(value, 1);
        public void OnShortcut3(InputAction.CallbackContext value) => OnShortcut(value, 2);
        public void OnShortcut4(InputAction.CallbackContext value) => OnShortcut(value, 3);
        public void OnShortcut5(InputAction.CallbackContext value) => OnShortcut(value, 4);
        public void OnShortcut6(InputAction.CallbackContext value) => OnShortcut(value, 5);
        public void OnShortcut7(InputAction.CallbackContext value) => OnShortcut(value, 6);
        public void OnShortcut8(InputAction.CallbackContext value) => OnShortcut(value, 7);
        public void OnShortcut9(InputAction.CallbackContext value) => OnShortcut(value, 8);
        public void OnShortcut10(InputAction.CallbackContext value) => OnShortcut(value, 9);

        private void OnShortcut(InputAction.CallbackContext value, int index)
        {
            if (value.performed)
            {
                UIManager.Instance.ShortcutTarget = UIManager.Instance.ShortcutInventory[index];
            }
        }
    }
}
