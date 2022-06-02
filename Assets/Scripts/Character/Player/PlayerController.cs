using TouhouPrideGameJam4.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouhouPrideGameJam4.Character.Player
{
    public class PlayerController : ACharacter
    {
        private void Awake()
        {
            Init();
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
                        TurnManager.Instance.MovePlayer(mov.x > 0 ? 1 : -1, 0);
                    }
                    else
                    {
                        TurnManager.Instance.MovePlayer(0, mov.y > 0 ? 1 : -1);
                    }
                }
            }
        }
    }
}
