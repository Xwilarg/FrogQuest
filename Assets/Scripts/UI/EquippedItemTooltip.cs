using TouhouPrideGameJam4.Character.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TouhouPrideGameJam4.UI
{
    public class EquippedItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            UIManager.Instance.Tooltip.gameObject.SetActive(true);
            UIManager.Instance.Tooltip.transform.position = transform.position - Vector3.down * ((RectTransform)UIManager.Instance.Tooltip.transform).sizeDelta.y;
            UIManager.Instance.Tooltip.Title.text = PlayerController.Instance.EquippedWeapon.Name;
            UIManager.Instance.Tooltip.Description.text = $"{PlayerController.Instance.EquippedWeapon.Description}\n\n<color=#555>{PlayerController.Instance.EquippedWeapon.UtilityDescription}";
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIManager.Instance.Tooltip.gameObject.SetActive(false);
        }
    }
}
