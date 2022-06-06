using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.SO.Item;
using TouhouPrideGameJam4.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.UI
{
    public class ShortcutButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnClick()
        {
            UIManager.Instance.ShortcutTarget = this;
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

        public Sprite ActionSprite => _content.ActionType.ActionSprite;

        public void Use()
        {
            try
            {
                var sound = _content.ActionType.ActionSound;
                _content.DoAction(PlayerController.Instance);
                SoundManager.Instance.PlayClip(sound);
                PlayerController.Instance.UpdateInventoryDisplay();
            }
            catch (NoFreeSpaceException)
            {
                SoundManager.Instance.PlayError();
            }
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

        public void OnPointerExit(PointerEventData eventData)
        {
            UIManager.Instance.Tooptip.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsEmpty)
            {
                UIManager.Instance.Tooptip.gameObject.SetActive(true);
                UIManager.Instance.Tooptip.transform.position = transform.position - Vector3.down * ((RectTransform)UIManager.Instance.Tooptip.transform).sizeDelta.y;
                UIManager.Instance.Tooptip.Title.text = _content.Name;
                UIManager.Instance.Tooptip.Description.text = $"{_content.Description}\n\n<color=#555>{_content.UtilityDescription}";
            }
        }
    }
}
