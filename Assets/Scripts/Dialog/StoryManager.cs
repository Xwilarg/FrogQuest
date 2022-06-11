using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TouhouPrideGameJam4.Dialog.Parsing;
using TouhouPrideGameJam4.SO;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouPrideGameJam4.Dialog
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _introDialog;
        private DialogStatement[] _introStatement;

        [SerializeField]
        private VNCharacterInfo[] _characters;

        private void Start()
        {
            _introStatement = Parse(_introDialog);
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
            string dialogue = null;
            ParsingExpectation exp = ParsingExpectation.Start;

            foreach (var m in Regex.Matches(file.text, "\\w+|\"[\\w\\s]*\"|\\n").Cast<Match>().Select(x => x.Value))
            {
                var match = m;
                if (m.StartsWith("\"")) match = match[1..];
                if (m.EndsWith("\"") && !m.EndsWith("\\\"")) match = match[..1];
                if (match == "\n")
                {
                    if (exp == ParsingExpectation.NewLine)
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
                    if (character != null && exp == ParsingExpectation.Start) // We are at the start and found a character info
                    {
                        currentCharacter = character;
                        exp = ParsingExpectation.Mood;
                    }
                    else if (exp == ParsingExpectation.Mood) // Next element, mood info
                    {
                        targetMood = match.ToLowerInvariant() switch
                        {
                            "happy" => currentCharacter.HappyExpression,
                            "neutral" => currentCharacter.NeutralExpression,
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
