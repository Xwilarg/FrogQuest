using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.SO;
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

        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }

        public void DisplayShop()
        {
            for (int i = 0; i < _shopContainer.childCount; i++) Destroy(_shopContainer.GetChild(i).gameObject);

            var items = PersistencyManager.Instance.BuyableItems;
            AddButton(items.Where(x => x.Item.Type == Inventory.ItemType.Potion), "potion");
            AddButton(items.Where(x => x.Item.Type == Inventory.ItemType.Spell), "spell");
            AddButton(items.Where(x => x.Item.Type == Inventory.ItemType.Weapon), "weapon");

            _energyText.text = $"Current energy: {PersistencyManager.Instance.TotalEnergy}";
        }

        private void AddButton(IEnumerable<BuyableItem> items, string name)
        {
            if (!items.Any())
            {
                return;
            }
            var go = Instantiate(_shopButtonPrefab, _shopContainer);
            if (PersistencyManager.Instance.TotalEnergy < 200)
            {
                go.GetComponent<Button>().interactable = false;
            }
            go.GetComponentInChildren<TMP_Text>().text = $"Random {name}\n\n200 energy";
        }
    }
}
