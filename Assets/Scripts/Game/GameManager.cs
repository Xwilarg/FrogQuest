using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Character;
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

        [SerializeField]
        private FollowerInfo _reimu, _aya;

        public StatusInfo GetStatusFromType(StatusType type)
            => _status.FirstOrDefault(x => x.Effect == type);

        public FollowerInfo FollowerReimu => _reimu;
        public FollowerInfo FollowerAya => _aya;
    }
}
