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
            TurnManager.Instance.UpdateDebugText();
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
    }
}
