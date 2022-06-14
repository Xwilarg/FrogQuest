using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Character;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private ShopInfo _buyableItems;

        [SerializeField]
        private AItemInfo[] _defaultUnlocked;

        private readonly List<AItemInfo> _findableInChest = new();

        private void Awake()
        {
            Instance = this;
            _findableInChest.AddRange(_defaultUnlocked);
        }

        public AItemInfo RandomUnlockedItem => _defaultUnlocked[Random.Range(0, _defaultUnlocked.Length)];

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
