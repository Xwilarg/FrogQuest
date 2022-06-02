using UnityEngine;

namespace TouhouPrideGameJam4.Character
{
    public class ACharacter : MonoBehaviour
    {
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
    }
}
