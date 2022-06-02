using System.Linq;
using TMPro;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _nameContainer, _descriptionContainer, _quantityContainer;

        [SerializeField]
        private GameObject _textPrefab;

        private AItemInfo[] _items;

        public void UpdateContent(AItemInfo[] items, ItemType? filter)
        {
            _items = items;

            for (var i = 0; i < _nameContainer.childCount; i++) Destroy(_nameContainer.GetChild(i).gameObject);
            for (var i = 0; i < _descriptionContainer.childCount; i++) Destroy(_descriptionContainer.GetChild(i).gameObject);
            for (var i = 0; i < _quantityContainer.childCount; i++) Destroy(_quantityContainer.GetChild(i).gameObject);
            foreach (var item in items.Where(x => filter == null || x.Type == filter.Value))
            {
                Instantiate(_textPrefab, _nameContainer).GetComponent<TMP_Text>().text = item.Name;
                Instantiate(_textPrefab, _descriptionContainer).GetComponent<TMP_Text>().text = item.Description;
                Instantiate(_textPrefab, _quantityContainer).GetComponent<TMP_Text>().text = "1";
            }
        }

        public void FilterAll() => UpdateContent(_items, null);
        public void FilterWeapons() => UpdateContent(_items, ItemType.Weapon);
        public void FilterConsumables() => UpdateContent(_items, ItemType.Consummable);
    }
}
