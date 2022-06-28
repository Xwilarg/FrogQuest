using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Character;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Game.Persistency
{
    public class PersistencyInit : MonoBehaviour
    {
        [SerializeField]
        private ShopInfo _buyableItems;

        [SerializeField]
        private AItemInfo[] _defaultUnlocked;

        [SerializeField]
        private FollowerInfo _reimu, _aya;

        public void Awake()
        {
            if (PersistencyManager.Instance == null)
            {
                var go = new GameObject("PersistencyManager", typeof(PersistencyManager));
                go.GetComponent<PersistencyManager>().Init(_buyableItems, _defaultUnlocked);
                DontDestroyOnLoad(go);
            }
        }

        public void SetReimu()
        {
            PersistencyManager.Instance.Follower = _reimu;
        }

        public void SetAya()
        {
            PersistencyManager.Instance.Follower = _aya;
        }
    }
}
