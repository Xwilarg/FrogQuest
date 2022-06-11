using System.Linq;
using System.Text.RegularExpressions;
using TouhouPrideGameJam4.SO;
using UnityEngine;

namespace TouhouPrideGameJam4.Dialog
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _introDialog;

        [SerializeField]
        private VNCharacterInfo[] _characters;

        private void Start()
        {
            Parse(_introDialog);
        }

        private void Parse(TextAsset file)
        {
            string text = file.text;

            VNCharacterInfo currentCharacter = null;

            while (text.Any())
            {
                var match = Regex.Match(text, "\\w+|\"[\\w\\s]*\"|\\n");
                if (match.Success)
                {
                    var next = match.Value;
                    text = text[..next.Length];
                    Debug.Log(next);
                }
                else
                {
                    break; // Nothing left to do I guess?
                }
            }
        }
    }
}
