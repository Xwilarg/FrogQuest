using TMPro;
using UnityEngine;

namespace TouhouPrideGameJam4.Effect
{
    public class TextFadeAway : MonoBehaviour
    {
        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.color = new Color(
                r: _text.color.r,
                g: _text.color.g,
                b: _text.color.b,
                a: _text.color.a - Time.deltaTime
            );
            if (_text.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
