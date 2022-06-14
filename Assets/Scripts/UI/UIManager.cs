using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField]
        private GameObject _statusPrefab;

        [SerializeField]
        private Transform _statusContainer;

        private float _baseHealth;
        [SerializeField]
        private Image _healthBar;

        [SerializeField]
        private Image _takeDropImage;

        [SerializeField]
        private Sprite _spriteTake, _spriteDrop, _spriteFull;

        [SerializeField]
        private GameObject _followerContainer;

        [SerializeField]
        private Image _followerImage;

        public Image ShortcutEquipped;
        public ShortcutButton[] ShortcutInventory;
        public Image ShortcutAction;
        public Sprite ActionNone;

        public Tooltip Tooltip;

        private ShortcutButton _shortcutTarget = null;

        private void Awake()
        {
            Instance = this;
            _baseHealth = _healthBar.rectTransform.sizeDelta.x;
        }

        private void SetFollower(Sprite image)
        {
            _followerContainer.SetActive(true);
            _followerImage.sprite = image;
        }

        public void SetHealth(float value)
        {
            _healthBar.rectTransform.sizeDelta = new Vector2(value * _baseHealth, _healthBar.rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Remove tint that indicate that an element is selected
        /// </summary>
        public void ResetHighlight()
        {
            foreach (var btn in ShortcutInventory)
            {
                btn.ClearHighlight();
            }
        }

        public void UseCurrent()
        {
            if (_shortcutTarget != null && !_shortcutTarget.IsEmpty)
            {
                _shortcutTarget.Use();
            }
            else
            {
                SoundManager.Instance.PlayError();
            }
        }

        public void DropTake()
        {
            var pos = PlayerController.Instance.Position;
            if (MapManager.Instance.IsAnythingOnFloor(pos.x, pos.y))
            {
                if (!ShortcutInventory.Any(x => x.IsEmpty))
                {
                    SoundManager.Instance.PlayError();
                }
                else
                {
                    PlayerController.Instance.AddItem(MapManager.Instance.TakeItemFromFloor(pos.x, pos.y));
                    UpdateUIOnNewTile();
                }
            }
            else if (_shortcutTarget != null && !_shortcutTarget.IsEmpty)
            {
                MapManager.Instance.SetItemOnFloor(pos.x, pos.y, _shortcutTarget.Content);
                PlayerController.Instance.RemoveItem(_shortcutTarget.Content);
                UpdateUIOnNewTile();
            }
            else
            {
                SoundManager.Instance.PlayError();
            }
        }

        public void UpdateUIOnNewTile()
        {
            var pos = PlayerController.Instance.Position;
            if (MapManager.Instance.IsAnythingOnFloor(pos.x, pos.y))
            {
                if (!ShortcutInventory.Any(x => x.IsEmpty))
                {
                    _takeDropImage.sprite = _spriteFull;
                }
                else
                {
                    _takeDropImage.sprite = _spriteTake;
                }
            }
            else if (_shortcutTarget != null && !_shortcutTarget.IsEmpty)
            {
                _takeDropImage.sprite = _spriteDrop;
            }
            else
            {
                _takeDropImage.sprite = ActionNone;
            }
        }

        public void UpdateStatus(IReadOnlyDictionary<StatusType, int> effects)
        {
            for (int i = 0; i < _statusContainer.childCount; i++) Destroy(_statusContainer.GetChild(i).gameObject);

            foreach (var e in effects)
            {
                var go = Instantiate(_statusPrefab, _statusContainer);
                go.GetComponent<Image>().sprite = GameManager.Instance.GetStatusFromType(e.Key).Sprite;
                go.GetComponentInChildren<TMP_Text>().text = e.Value.ToString();
            }
        }

        /// <summary>
        /// Element currently selected in the action bar
        /// </summary>
        public ShortcutButton ShortcutTarget
        {
            set
            {
                ResetHighlight();
                if (value == null)
                {
                    ShortcutAction.sprite = ActionNone;
                }
                else
                {
                    value.SetHighlight();
                    ShortcutAction.sprite = value.IsEmpty ? ActionNone : value.ActionSprite;
                    SoundManager.Instance.PlaySelectBip();
                }
                _shortcutTarget = value;
                UpdateUIOnNewTile();
            }
            get => _shortcutTarget;
        }
    }
}
