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

        /// <summary>
        /// Element currently selected in the action bar
        /// </summary>
        public ShortcutButton ShortcutTarget
        {
            set
            {
                ResetHighlight();
                if (value != null)
                {
                    value.SetHighlight();
                }
                _shortcutTarget = value;
            }
            get => _shortcutTarget;
        }
        private ShortcutButton _shortcutTarget = null;

        public Image ShortcutEquipped;
        public ShortcutButton[] ShortcutInventory;
    }
}
