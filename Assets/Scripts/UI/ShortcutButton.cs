using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.UI
{
    public class ShortcutButton : MonoBehaviour
    {
        public void OnClick()
        {
            if (_content != null)
            {
                UIManager.Instance.ShortcutTarget = this;
            }
        }

        /// <summary>
        /// What is actually in the tile (the object the player has set there)
        /// </summary>
        private Image _contentImage;
        /// <summary>
        /// Highlight color indicating if the item is selected
        /// </summary>
        private Image _highlightImage;
        /// <summary>
        /// Item that is contained there
        /// </summary>
        private AItemInfo _content;

        public bool IsEmpty => _content == null;

        private void Awake()
        {
            _contentImage = transform.GetChild(0).GetComponent<Image>();
            _highlightImage = GetComponent<Image>();
        }

        public Sprite ActionSprite => _content.ActionSprite;

        public void Use()
        {
            _content.DoAction(PlayerController.Instance);
            PlayerController.Instance.UpdateInventoryDisplay();
        }

        public void SetHighlight()
        {
            _highlightImage.color = Color.yellow;
        }

        public void ClearHighlight()
        {
            _highlightImage.color = new Color(1f, 1f, 1f, .4f);
        }

        public void Clear()
        {
            SetContent(null);
        }

        public void SetContent(AItemInfo item)
        {
            if (item != _content)
            {
                UIManager.Instance.ShortcutTarget = null;
            }
            _content = item;
            _contentImage.sprite = item != null ? item.Sprite : null;
            _contentImage.color = item == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
        }
    }
}
