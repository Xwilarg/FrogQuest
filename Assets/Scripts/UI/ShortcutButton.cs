using TouhouPrideGameJam4.SO.Item;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.UI
{
    public class ShortcutButton : MonoBehaviour
    {
        public void OnClick()
        { }

        private Image _contentImage;
        private AItemInfo _content;

        private void Awake()
        {
            _contentImage = transform.GetChild(0).GetComponent<Image>();
        }

        public void ClearColor()
        {
            _contentImage.color = new Color(0f, 0f, 0f, 0f);
        }

        public void SetContent(AItemInfo item)
        {
            _content = item;
            _contentImage.sprite = item?.Sprite;
            _contentImage.color = item == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
        }
    }
}
