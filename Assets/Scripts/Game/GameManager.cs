using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.SO;
using UnityEngine;

namespace TouhouPrideGameJam4.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private StatusInfo[] _status;

        public StatusInfo GetStatusFromType(StatusType type)
            => _status.FirstOrDefault(x => x.Effect == type);
    }
}
