using System.Collections.Generic;
using System.Linq;
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

        public List<BuyableItem> BuyableItems { get; } = new();
        private readonly List<AItemInfo> _unlocked = new();

        public void UnlockItem(AItemInfo item)
        {
            _unlocked.Add(item);
            BuyableItems.RemoveAll(x => x.Item.Name == item.Name);
        }

        public AItemInfo RandomUnlockedItem => _unlocked[Random.Range(0, _unlocked.Count)];

        public void Init(ShopInfo buyableItems, AItemInfo[] defaultUnlocked)
        {
            BuyableItems.AddRange(buyableItems.Items.ToList());
            _unlocked.AddRange(defaultUnlocked);
        }
    }
}
