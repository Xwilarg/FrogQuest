using System.Collections.Generic;
using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Game.Persistency
{
    public class PersistencyManager : MonoBehaviour
    {
        public static PersistencyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private ShopInfo _buyableItems;
        private readonly List<AItemInfo> _unlocked = new();

        public AItemInfo RandomUnlockedItem => _unlocked[Random.Range(0, _unlocked.Count)];

        public void Init(ShopInfo buyableItems, AItemInfo[] defaultUnlocked)
        {
            _buyableItems = buyableItems;
            _unlocked.AddRange(defaultUnlocked);
        }
    }
}
