using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Dialog.Parsing;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.SO.Character;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.Dialog
{
    public class StoryManager : MonoBehaviour
    {
        public static StoryManager Instance { get; private set; }

        [SerializeField]
        private TMP_Text _vnName, _vnContent;

        [SerializeField]
        private Image _vnImage;

        [SerializeField]
        private GameObject _vnContainer;

        [SerializeField]
        private TextAsset _introDialog;
        private DialogStatement[] _introStatement;

        private DialogStatement[] _current;
        private int _index;

        [SerializeField]
        private VNCharacterInfo[] _characters;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (PersistencyManager.Instance.StoryProgress == StoryProgress.Intro)
            {
                _introStatement = Parse(_introDialog);
                PersistencyManager.Instance.IncreaseStory();
            }

            ReadIntroduction();
        }

        public void ShowNextDialogue()
        {
            if (_index == _current.Length || _current == null) // End of VN part
            {
                _vnContainer.SetActive(false);
                PlayerController.Instance.EnableRPGController();
                _current = null;
            }
            else
            {
                _vnName.text = _current[_index].Name;
                _vnContent.text = _current[_index].Content;
                if (_current[_index].Image == null)
                {
                    _vnImage.gameObject.SetActive(false);
                }
                else
                {
                    _vnImage.gameObject.SetActive(true);
                    _vnImage.sprite = _current[_index].Image;
                }
                _index++;
            }
        }

        private void ReadDialogues(DialogStatement[] toRead)
        {
            PlayerController.Instance.EnableVNController();
            _vnContainer.SetActive(true);
            _current = toRead;
            _index = 0;
            ShowNextDialogue();
        }

        public void ReadIntroduction()
        {
            ReadDialogues(_introStatement);
        }

        private enum ParsingExpectation
        {
            Dialogue,
            Mood,
            Start,
            NewLine
        }

        private DialogStatement[] Parse(TextAsset file)
        {
            string text = file.text;

            List<DialogStatement> lines = new();
            Sprite targetMood = null;
            VNCharacterInfo currentCharacter = null;
            ParsingExpectation exp = ParsingExpectation.Start;

            foreach (var m in Regex.Matches(file.text, "\\w+|\"[\\w\\s!?'…,.]*\"|\\n").Cast<Match>().Select(x => x.Value))
            {
                var match = m;
                if (m.StartsWith("\"")) match = match[1..];
                if (m.EndsWith("\"") && !m.EndsWith("\\\"")) match = match[..^1];
                if (match == "\n")
                {
                    if (exp == ParsingExpectation.NewLine || exp == ParsingExpectation.Start)
                    {
                        exp = ParsingExpectation.Start;
                    }
                    else
                    {
                        throw new System.InvalidOperationException($"Parsing of {file.name} failed at {match} for state {exp}");
                    }
                }
                else
                {
                    var character = _characters.FirstOrDefault(x => x.Key == match.ToLowerInvariant());
                    if (match == "none" && exp == ParsingExpectation.Start) // Unset character
                    {
                        currentCharacter = null;
                        exp = ParsingExpectation.Dialogue;
                    }
                    else if (character != null && exp == ParsingExpectation.Start) // We are at the start and found a character info
                    {
                        currentCharacter = character;
                        exp = ParsingExpectation.Mood;
                    }
                    else if (exp == ParsingExpectation.Mood) // Next element, mood info
                    {
                        targetMood = match.ToLowerInvariant() switch
                        {
                            "joyful" => currentCharacter.JoyfulExpression,
                            "neutral" => currentCharacter.NeutralExpression,
                            "eyesclosed" => currentCharacter.EyesClosedExpression,
                            "angry" => currentCharacter.AngryExpression,
                            "surprised" => currentCharacter.SurprisedExpression,
                            "sad" => currentCharacter.SadExpression,
                            _ => throw new System.InvalidOperationException($"Invalid expression {match}")
                        };
                        exp = ParsingExpectation.Dialogue;
                    }
                    else if (character == null)
                    {
                        lines.Add(new()
                        {
                            Name = currentCharacter != null ? currentCharacter.Name : null,
                            Image = currentCharacter != null ? targetMood : null,
                            Content = match
                        });
                        exp = ParsingExpectation.NewLine;
                    }
                    else
                    {
                        throw new System.InvalidOperationException($"Parsing of {file.name} failed at {match} for state {exp}");
                    }
                }
            }

            return lines.ToArray();
        }
    }
}
