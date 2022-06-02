using TouhouPrideGameJam4.Game;
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
            Init();
            _source = GetComponent<AudioSource>();
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
