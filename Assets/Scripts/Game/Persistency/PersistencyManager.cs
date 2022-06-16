using System.Collections.Generic;
using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TouhouPrideGameJam4.Game.Persistency
{
    public class PersistencyManager : MonoBehaviour
    {
        public static PersistencyManager Instance { get; private set; }

        public int TotalEnergy { set; get; }

        public StoryProgress StoryProgress { private set; get; }

        private void Awake()
        {
            Instance = this;
            SceneManager.activeSceneChanged += (_, _2) =>
            {
                SceneManager.LoadScene("VNUI", LoadSceneMode.Additive);
            };
        }

        public void IncreaseStory()
        {
            StoryProgress++;
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
