using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.Menu
{
    public class RunMenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _shopButtonPrefab;

        [SerializeField]
        private Transform _shopContainer;

        [SerializeField]
        private TMP_Text _energyText;

        [SerializeField]
        private Button _reimuB, _ayaB;

        [SerializeField]
        private Button _easyButton;

        public void TriggerEasyMode()
        {
            PersistencyManager.Instance.EasyMode = true;
            PersistencyManager.Instance.TotalEnergy += 1000;
        }

        private void Start()
        {
            _ayaB.gameObject.SetActive(PersistencyManager.Instance.StoryProgress > StoryProgress.YoukaiMountain1);
            _reimuB.gameObject.SetActive(PersistencyManager.Instance.StoryProgress > StoryProgress.YoukaiMountain1Half);

            if (PersistencyManager.Instance.Follower != null)
            {
                if (PersistencyManager.Instance.Follower.Type == FollowerType.Aya)
                {
                    _ayaB.interactable = false;
                }
                else
                {
                    _reimuB.interactable = false;
                }
            }

            if (PersistencyManager.Instance.EasyMode)
            {
                _easyButton.interactable = false;
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }

        public void DisplayShop()
        {
            for (int i = 0; i < _shopContainer.childCount; i++) Destroy(_shopContainer.GetChild(i).gameObject);

            AddButton(ItemType.Potion);
            AddButton(ItemType.Spell);
            AddButton(ItemType.Weapon);
            AddButton(ItemType.BonusChest);

            _energyText.text = $"Current energy: {PersistencyManager.Instance.TotalEnergy}";
        }

        private void AddButton(ItemType type)
        {
            int price = PersistencyManager.Instance.GetPrice(type);
            if (type != ItemType.BonusChest && !PersistencyManager.Instance.BuyableItems.Any(x => x.Item.Type == type))
            {
                return;
            }
            var go = Instantiate(_shopButtonPrefab, _shopContainer);
            if (PersistencyManager.Instance.TotalEnergy < price)
            {
                go.GetComponent<Button>().interactable = false;
            }
            else
            {
                go.GetComponent<Button>().onClick.AddListener(new(() =>
                {
                    PersistencyManager.Instance.TotalEnergy -= price;
                    if (type == ItemType.BonusChest)
                    {
                        PersistencyManager.Instance.BonusChestCount++;
                    }
                    else
                    {
                        var list = PersistencyManager.Instance.BuyableItems.Where(x => x.Item.Type == type).ToArray();
                        var item = list[Random.Range(0, list.Length)];
                        PersistencyManager.Instance.UnlockItem(item.Item);
                    }
                    DisplayShop();
                }));
            }
            if (type == ItemType.BonusChest)
            {
                go.GetComponentInChildren<TMP_Text>().text = $"+1 chest per level\n\n{price} energy";
            }
            else
            {
                go.GetComponentInChildren<TMP_Text>().text = $"Random {type.ToString().ToLowerInvariant()}\n\n{price} energy";
            }
        }
    }
}
