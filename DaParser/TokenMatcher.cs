using System.Text.RegularExpressions;

namespace EventScript
{
    public class TokenMatcher
        {
            public TokenType Type { get; private set; }
            public string MatchValue { get; private set; }

            private Regex pattern;

            public TokenMatcher(TokenType type, string patternString)
            {
                Type = type;
                pattern = new Regex(patternString);
            }

            public bool IsMatch(string input)
            {
                Match match = pattern.Match(input);
                if (match.Success)
                {
                    MatchValue = match.Value;
                }
                return match.Success;
            }
        }
    }
