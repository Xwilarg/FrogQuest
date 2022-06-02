using System;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _nameContainer, _descriptionContainer, _quantityContainer, _actionContainer;

        [SerializeField]
        private GameObject _textPrefab, _buttonPrefab;

        private AItemInfo[] _items;
        private ACharacter _owner;

        public void UpdateContent(ACharacter owner, AItemInfo[] items, ItemType? filter)
        {
            _owner = owner;
            _items = items;

            for (var i = 0; i < _nameContainer.childCount; i++) Destroy(_nameContainer.GetChild(i).gameObject);
            for (var i = 0; i < _descriptionContainer.childCount; i++) Destroy(_descriptionContainer.GetChild(i).gameObject);
            for (var i = 0; i < _quantityContainer.childCount; i++) Destroy(_quantityContainer.GetChild(i).gameObject);
            for (var i = 0; i < _actionContainer.childCount; i++) Destroy(_actionContainer.GetChild(i).gameObject);
            foreach (var item in items.Where(x => filter == null || x.Type == filter.Value))
            {
                Instantiate(_textPrefab, _nameContainer).GetComponent<TMP_Text>().text = item.Name;
                Instantiate(_textPrefab, _descriptionContainer).GetComponent<TMP_Text>().text = item.Description;
                Instantiate(_textPrefab, _quantityContainer).GetComponent<TMP_Text>().text = "1";
                var button = Instantiate(_buttonPrefab, _actionContainer);
                button.GetComponentInChildren<TMP_Text>().text = item.Type switch
                {
                    ItemType.Weapon => "Equip",
                    ItemType.Consummable => "Use",
                    _ => throw new NotImplementedException()
                };
                button.GetComponent<Button>().onClick.AddListener(new(() =>
                {
                    switch (item.Type)
                    {
                        case ItemType.Weapon:
                            break; // TODO

                        case ItemType.Consummable:
                            var consumable = (ConsumableInfo)item;
                            switch (consumable.Effect)
                            {
                                case EffectType.Heal:
                                    _owner.TakeDamage(-consumable.Value);
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }));
            }
        }

        public void FilterAll() => UpdateContent(_owner, _items, null);
        public void FilterWeapons() => UpdateContent(_owner, _items, ItemType.Weapon);
        public void FilterConsumables() => UpdateContent(_owner, _items, ItemType.Consummable);
    }
}
