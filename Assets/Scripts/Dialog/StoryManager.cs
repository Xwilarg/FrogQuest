using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Dialog.Parsing;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.Map;
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
        private TextAsset _introDialog, _mountain1, _mountain2, _forest1, _forest2, _endQuestAya, _endQuestReimu, _questAya, _questReimu;
        private DialogStatement[] _introStatement, _mountain1Statement, _mountain2Statement, _forest1Statement, _forest2Statement, _endQuestAyaStatement, _endQuestReimuStatement, _questAyaStatement, _questReimuStatement;

        private DialogStatement[] _current;
        private int _index;

        [SerializeField]
        private VNCharacterInfo[] _characters;

        [SerializeField]
        private GameObject _skipIcon;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var c = _characters.ToList();
            c.Add(new()
            {
                Name = "???",
                Key = "???"
            });
            _characters = c.ToArray();

            ParseAllStories();
            ProgressIsAvailable(StoryProgress.Intro);
            if (MapManager.Instance != null)
            {
                ProgressIsAvailable(StoryProgress.YoukaiMountain1);
            }
        }

        public void ParseAllStories()
        {
            _introStatement = Parse(_introDialog);
            _mountain1Statement = Parse(_mountain1);
            _mountain2Statement = Parse(_mountain2);
            _forest1Statement = Parse(_forest1);
            _forest2Statement = Parse(_forest2);
            _endQuestAyaStatement = Parse(_endQuestAya);
            _endQuestReimuStatement = Parse(_endQuestReimu);
            _questAyaStatement = Parse(_questAya);
            _questReimuStatement = Parse(_questReimu);
        }

        private bool _isSkipping;
        public void ToggleSkipDialogs()
        {
            _isSkipping = !_isSkipping;
            _skipIcon.SetActive(_isSkipping);
            if (_isSkipping)
            {
                StartCoroutine(SkipDialogues());
            }
        }

        private IEnumerator SkipDialogues()
        {
            while (_isSkipping)
            {
                ShowNextDialogue();
                yield return new WaitForSeconds(.1f);
            }
        }

        public void ProgressIsAvailable(StoryProgress requirement)
        {
            if (PersistencyManager.Instance.StoryProgress == requirement)
            {
                ReadDialogues(requirement switch
                {
                    StoryProgress.Intro => _introStatement,
                    StoryProgress.YoukaiMountain1 => _mountain1Statement,
                    StoryProgress.YoukaiMountain1Half => _mountain2Statement,
                    StoryProgress.Forest1 => _forest1Statement,
                    StoryProgress.Forest4Kill => _forest2Statement,
                    _ => throw new NotImplementedException()
                });
                PersistencyManager.Instance.IncreaseStory();
            }
        }

        public void ShowNextDialogue()
        {
            if (_current == null || _index == _current.Length) // End of VN part
            {
                _vnContainer.SetActive(false);
                PlayerController.Instance.EnableRPGController();
                _current = null;
                foreach (var button in GameObject.FindGameObjectsWithTag("MenuButton").Select(x => x.GetComponent<Button>()))
                {
                    button.interactable = true;
                }
                _isSkipping = false;
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
            foreach (var button in GameObject.FindGameObjectsWithTag("MenuButton").Select(x => x.GetComponent<Button>()))
            {
                button.interactable = false;
            }
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

            foreach (var m in Regex.Matches(file.text, "[\\w?]+|\"[\\w\\s!?'’…,.()…‘’:-]*\"|\\n").Cast<Match>().Select(x => x.Value))
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
                        throw new InvalidOperationException($"Parsing of {file.name} failed at {match} for state {exp}");
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
                    else if (match == "???" && exp == ParsingExpectation.Start) // We are at the start and found a character info
                    {
                        currentCharacter = character;
                        exp = ParsingExpectation.Mood;
                    }
                    else if (character != null && exp == ParsingExpectation.Start) // We are at the start and found a character info
                    {
                        currentCharacter = character;
                        exp = ParsingExpectation.Mood;
                    }
                    else if (exp == ParsingExpectation.Mood) // Next element, mood info
                    {
                        if (currentCharacter.Key != "???")
                        {
                            targetMood = match.ToLowerInvariant() switch
                            {
                                "joyful" => currentCharacter.JoyfulExpression,
                                "neutral" => currentCharacter.NeutralExpression,
                                "eyesclosed" => currentCharacter.EyesClosedExpression,
                                "angry" => currentCharacter.AngryExpression,
                                "surprised" => currentCharacter.SurprisedExpression,
                                "sad" => currentCharacter.SadExpression,
                                "shocked" => currentCharacter.ShockedExpression,
                                "shy" => currentCharacter.ShyExpression,
                                _ => throw new InvalidOperationException($"Invalid expression {match}")
                            };
                        }
                        else
                        {
                            targetMood = null;
                        }
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
