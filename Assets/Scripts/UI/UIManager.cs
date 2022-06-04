using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _source = GetComponent<AudioSource>();
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
                PlaySound(ClipNone);
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
                }
                _shortcutTarget = value;
            }
            get => _shortcutTarget;
        }

        public void PlaySound(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }

        private ShortcutButton _shortcutTarget = null;

        public Image ShortcutEquipped;
        public ShortcutButton[] ShortcutInventory;
        public Image ShortcutAction;
        public Sprite ActionNone;
        public AudioClip ClipNone;

        private AudioSource _source;
    }
}
