using TouhouPrideGameJam4.Character.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TouhouPrideGameJam4.UI
{
    public class EquippedItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PlayerController.Instance.CanAttack())
            {
                UIManager.Instance.Tooptip.gameObject.SetActive(true);
                UIManager.Instance.Tooptip.transform.position = transform.position - Vector3.down * ((RectTransform)UIManager.Instance.Tooptip.transform).sizeDelta.y;
                UIManager.Instance.Tooptip.Title.text = PlayerController.Instance.EquipedWeapon.Name;
                UIManager.Instance.Tooptip.Description.text = $"{PlayerController.Instance.EquipedWeapon.Description}\n\n<color=#555>{PlayerController.Instance.EquipedWeapon.UtilityDescription}";
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIManager.Instance.Tooptip.gameObject.SetActive(false);
        }
    }
}
