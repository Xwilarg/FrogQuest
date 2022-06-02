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

        public void UpdateContent(AItemInfo[] items)
        {
            while (_nameContainer.childCount > 0) Destroy(_nameContainer.GetChild(0).gameObject);
            while (_descriptionContainer.childCount > 0) Destroy(_descriptionContainer.GetChild(0).gameObject);
            while (_quantityContainer.childCount > 0) Destroy(_quantityContainer.GetChild(0).gameObject);
            foreach (var item in items)
            {
                Instantiate(_textPrefab, _nameContainer).GetComponent<TMP_Text>().text = item.Name;
                Instantiate(_textPrefab, _descriptionContainer).GetComponent<TMP_Text>().text = item.Description;
                Instantiate(_textPrefab, _quantityContainer).GetComponent<TMP_Text>().text = "1";
            }
        }
    }
}
