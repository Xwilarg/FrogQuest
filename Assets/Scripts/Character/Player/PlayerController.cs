using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouhouPrideGameJam4.Character.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private AudioClip _stepSound;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Init();
        }

        protected override void UpdateInventoryDisplay()
        {
            UIManager.Instance.ShortcutEquipped.sprite = _equipedWeapon.Sprite;
            UIManager.Instance.ShortcutEquipped.color = UIManager.Instance.ShortcutEquipped.sprite == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
            int index = 0;
            foreach (var img in UIManager.Instance.ShortcutInventory)
            {
                img.color = new Color(0f, 0f, 0f, 0f);
            }
            foreach (var item in _items)
            {
                if (item.Key == _equipedWeapon)
                {
                    continue;
                }

                UIManager.Instance.ShortcutInventory[index].sprite = item.Key.Sprite;
                UIManager.Instance.ShortcutInventory[index].color = Color.white;
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
    }
}
