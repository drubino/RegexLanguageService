using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegexLanguageService.Parser
{
    public static class RegexParser
    {
        private const string regexString = @"\[\^?]?(?:[^\\\]]+|\\[\S\s]?)*]?|\\(?:0(?:[0-3][0-7]{0,2}|[4-7][0-7]?)?|[1-9][0-9]*|x[0-9A-Fa-f]{2}|u[0-9A-Fa-f]{4}|c[A-Za-z]|[\S\s]?)|\((?:\?[:=!]?)?|(?:[?*+]|\{[0-9]+(?:,[0-9]*)?\})\??|[^.?*+^${[()|\\]+|.";
        private const string characterClassToken = @"[^\\-]+|-|\\(?:[0-3][0-7]{0,2}|[4-7][0-7]?|x[0-9A-Fa-f]{2}|u[0-9A-Fa-f]{4}|c[A-Za-z]|[\S\s]?)";
        private const string characterClassParts = @"^(\[\^?)(]?(?:[^\\\]]+|\\[\S\s]?)*)(]?)$";
        private const string quantifier = @"^(?:[?*+]|\{[0-9]+(?:,[0-9]*)?\})\??$";

        private static Regex regularExpressionRegex = new Regex(regexString, RegexOptions.Compiled);
        private static Regex characterClassTokenRegex = new Regex(characterClassToken, RegexOptions.Compiled);
        private static Regex characterClassPartsRegex = new Regex(characterClassParts, RegexOptions.Compiled);
        private static Regex quantifierRegex = new Regex(quantifier, RegexOptions.Compiled);

        private static string[] anchorTags = new[]
        {
            "^",
            "$"
        };

        private static string[] escapeCharacters = new[]
        {
            @"\t",
            @"\n",
            @"\r",
            @"\f",
            @"\cX",
            @"\N",
            @"\NNN",
            @"\b",
            @"\B",
            @"\d",
            @"\D",
            @"\s",
            @"\S",
            @"\w",
            @"\W",
            @"\Q",
            @"\U",
            @"\L",
            @"\[",
            @"\\",
            @"\^",
            @"\$",
            @"\.",
            @"\|",
            @"\?",
            @"\*",
            @"\+",
            @"\(",
            @"\)",
            @"\{",
            @"\}"
        };

        public static IEnumerable<RegexToken> Parse(string regexPattern)
        {
            var regexTokens = new List<RegexToken>();
            var matches = regularExpressionRegex.Matches(regexPattern).Cast<Match>().ToList();
            var captureGroupStringBuilder = new StringBuilder();
            var openParenthesisCount = 0;
            var inCaptureGroup = false;
            foreach (var match in matches)
            {
                var stringValue = match.Value;
                if (inCaptureGroup && stringValue != ")")
                {
                    if (stringValue.StartsWith("("))
                        openParenthesisCount++;

                    captureGroupStringBuilder.Append(stringValue);
                    continue;
                }

                if (stringValue.StartsWith("["))
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.RegexCharacterClass));
                }
                else if (quantifierRegex.IsMatch(stringValue))
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.RegexQuantifier));
                }
                else if (anchorTags.Contains(stringValue))
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.RegexAnchor));
                }
                else if (escapeCharacters.Contains(stringValue))
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.RegexEscapeCharacter));
                }
                else if (stringValue == "|")
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.RegexAlternation));
                }
                else if (stringValue.StartsWith("("))
                {
                    inCaptureGroup = true;
                    openParenthesisCount++;
                    captureGroupStringBuilder.Append(stringValue);
                }
                else if (stringValue == ")")
                {
                    openParenthesisCount--;
                    captureGroupStringBuilder.Append(stringValue);
                    if (openParenthesisCount == 0)
                    {
                        regexTokens.Add(new RegexToken(captureGroupStringBuilder.ToString(), RegexTokenType.RegexCaptureGroup));
                        inCaptureGroup = false;
                        captureGroupStringBuilder.Clear();
                    }
                }
                else
                {
                    regexTokens.Add(new RegexToken(stringValue, RegexTokenType.Default));
                }
            }

            return regexTokens;
        }

        #region Utilities

        private static IEnumerable<Match> GetMatches(Regex regex, string input)
        {
            return regex.Matches(input).Cast<Match>().ToList();
        }

        #endregion //Utilities
    }
}
