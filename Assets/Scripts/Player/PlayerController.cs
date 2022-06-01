using TouhouPrideGameJam4.Map;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouhouPrideGameJam4.Player
{
    public class PlayerController : MonoBehaviour
    {
        public void OnMovement(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                var mov = value.ReadValue<Vector2>();
                if (mov.x != 0f || mov.y != 0f)
                {
                    if (Mathf.Abs(mov.x) > Mathf.Abs(mov.y))
                    {
                        transform.position = (Vector2)MapManager.Instance.MovePlayer(mov.x > 0 ? 1 : -1, 0);
                    }
                    else
                    {
                        transform.position = (Vector2)MapManager.Instance.MovePlayer(0, mov.y > 0 ? 1 : -1);
                    }
                }
            }
        }
    }
}
