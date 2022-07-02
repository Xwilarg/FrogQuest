using System.Collections;
using System.Collections.Generic;
using TouhouPrideGameJam4.Character.Player;
using UnityEngine.EventSystems;
using UnityEngine;

namespace TouhouPrideGameJam4.UI
{
    public class WaitButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            UIManager.Instance.Tooltip.gameObject.SetActive(true);
            UIManager.Instance.Tooltip.transform.position = transform.position - Vector3.down * ((RectTransform)UIManager.Instance.Tooltip.transform).sizeDelta.y;
            UIManager.Instance.Tooltip.Title.text = "Wait";
            UIManager.Instance.Tooltip.Description.text = $"\n\n<color=#555>Pass a turn";
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIManager.Instance.Tooltip.gameObject.SetActive(false);
        }
    }
}