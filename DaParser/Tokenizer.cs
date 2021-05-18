using System.Collections.Generic;
using System.Text;

namespace EventScript
{
    public class Tokenizer : ErrorRaiser
    {
        private const char STRING_DELIMETER = '\'';
        private const char EQUALSIGN = '=';
        private const char ARROWRIGHT = '>';
        private const char ARROWLEFT = '<';

        private string sourceString;

        private StringBuilder sBuilder = new StringBuilder();

        private char? currentChar;

        private Token currentToken;

        private int currentLine = 1;
        private int currentColumn = 1;

        private int index = 0;

        private List<TokenMatcher> tokenMatchers = new List<TokenMatcher>()
        {
            //Dialogue specific
            { new TokenMatcher(TokenType.MEMBERDELIMITER_LEFT, @"(\{)")},
            { new TokenMatcher(TokenType.MEMBERDELIMITER_RIGHT, @"(\})")},
            { new TokenMatcher(TokenType.TRANSFER, @"(\=>)" )},

            //base
            
            { new TokenMatcher(TokenType.STRING, @"(?<=\')(.?)*(?=\')")},
            { new TokenMatcher(TokenType.FUNC, @"\b(function)\b")},

            { new TokenMatcher(TokenType.L_BLOCK, @"(\[)")},
            { new TokenMatcher(TokenType.R_BLOCK, @"(\])")},
            { new TokenMatcher(TokenType.COMMA, @"(\,)")},

            { new TokenMatcher(TokenType.EQUALS, @"(\==)")},
            { new TokenMatcher(TokenType.ASSIGN, @"(\=)")},
            { new TokenMatcher(TokenType.PLUS, @"(\+)")},
            { new TokenMatcher(TokenType.SEMI, @"(\;)")},
            { new TokenMatcher(TokenType.MINUS, @"(\-)")},
            { new TokenMatcher(TokenType.MUL, @"(\*)")},
            { new TokenMatcher(TokenType.DIV, @"(\/)")},
            { new TokenMatcher(TokenType.SMALLER, @"(\<)")},
            { new TokenMatcher(TokenType.GREATER, @"(\>)")},
            { new TokenMatcher(TokenType.SMALLEREQUALS, @"(\<=)")},
            { new TokenMatcher(TokenType.GREATEREQUALS, @"(\>=)")},


            { new TokenMatcher(TokenType.EOF, @"(\0)")},
            { new TokenMatcher(TokenType.ID, @"\b((?i)[a-aZ-z_][a-aZ-z0-9_]*)\b")},
            { new TokenMatcher(TokenType.L_PAREN, @"(\()")},
            { new TokenMatcher(TokenType.NUMBER, @"\d+")},
            { new TokenMatcher(TokenType.R_PAREN, @"(\))")},
            { new TokenMatcher(TokenType.EOF, @"(\!)")},
        };
        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            //Dialogue specific
            { "Text", TokenType.TEXT_MEMBER },
            { "Choice", TokenType.CHOICE_MEMBER },

            //base
            { "if", TokenType.CONDITION },
            { "else", TokenType.ELSE },
            { "elif", TokenType.ELSEIF },
            { "dialogue", TokenType.DIALOGUESCRIPT },
            { "quest", TokenType.QUESTCRIPT },
            { "end",  TokenType.END },
            { "then",  TokenType.THEN },
            { "string", TokenType.TYPESPEC },
            { "int", TokenType.TYPESPEC },
            { "double", TokenType.TYPESPEC },
            { "bool", TokenType.TYPESPEC },
        };

        /// <summary>
        /// Returns a list of tokens from input string
        /// </summary>
        /// <param name="input"></param>
        public List<Token> Tokenize(string input)
        {
            sourceString = input;

            List<Token> result = new List<Token>();

            currentChar = GetNextChar(sourceString);

            sBuilder = new StringBuilder();

            while (currentChar != null)
            {
                switch (currentChar)
                {
                    case '\r':
                        break;
                    case '\n':
                        currentLine++;
                        currentColumn = 1;
                        break;
                    case ' ':
                        break;
                    case '(':
                    case ')':
                        StartCreateNewToken(currentLine, currentColumn);
                        result.Add(Create(currentChar.Value));
                        break;

                    case '[':
                    case ']':
                        StartCreateNewToken(currentLine, currentColumn);
                        result.Add(Create(currentChar.Value));
                        break;
                    case '-':
                    case '+':
                        StartCreateNewToken(currentLine, currentColumn);
                        result.Add(Create(currentChar.ToString()));
                        break;
                    case ';':
                        StartCreateNewToken(currentLine, currentColumn);
                        result.Add(Create(currentChar.ToString()));
                        break;
                    default:
                        StartCreateNewToken(currentLine, currentColumn);
                        result.Add(Create(CreateIDString(sourceString)));
                        break;
                }

                currentChar = GetNextChar(sourceString);
            }
            StartCreateNewToken(currentLine, currentColumn);
            result.Add(Create(currentChar));

            return result;
        }

        /// <summary>
        /// Increases the inspected index and returns the char at that index from the input string
        /// </summary>
        /// <returns></returns>
        private char? GetNextChar(string input)
        {
            index++;
            currentColumn++;

            if (index < input.Length)
            {
                return input[index];
            }
            return null;
        }

        /// <summary>
        /// Returns the char at index +1 from the input string
        /// </summary>
        /// <returns></returns>
        private char? PeekNextChar(string input)
        {
            int idx = index + 1;
            if (idx < input.Length)
                return input[idx];

            return null;
        }

        private bool IsKeyword(string input)
        {
            return keywords.ContainsKey(input);
        }

        /// <summary>
        /// Closes the Token creation process and sets its type and value from a predefined keyword
        /// </summary>
        /// <param name="input"></param>
        private Token EndCreateKeywordToken(string input)
        {
            TokenType type = keywords[input];
            return EndCreateNewToken(type, input);
        }

        /// <summary>
        /// Creates an empty Token without type and value at line and column
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        private void StartCreateNewToken(int line, int column)
        {
            currentToken = new Token();
            currentToken.SetPosition(line, column);
        }

        /// <summary>
        /// Closes the Token creation process and sets its type and value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Token EndCreateNewToken(TokenType type, string value)
        {
            currentToken.SetType(type);
            currentToken.SetValue(value);
            sBuilder.Clear();
            return currentToken;
        }

        /// <summary>
        /// Creates a string of alphanumerical chars
        /// </summary>
        /// <param name="sBuilder"></param>
        private string CreateIDString(string input)
        {
            char? itrChar = currentChar;
            sBuilder.Append(itrChar.Value);

            //prevent IDs from starting with a digit 
            if (char.IsDigit(itrChar.Value))
            {
                itrChar = PeekNextChar(sourceString);
                while (char.IsDigit(itrChar.Value))
                {
                    Advance();
                }

                //if (char.IsLetter(itrChar.Value))
                //    Error(LexicalError.InvalidVariableName);
            }
            else if (char.IsLetter(itrChar.Value))
            {
                itrChar = PeekNextChar(sourceString);
                while (char.IsLetterOrDigit(itrChar.Value))
                {
                    Advance();
                }
            }
            else if (itrChar.Value == STRING_DELIMETER)
            {
                /* A string is a STRING_DELIMETER followed by an Id followed by a STRING_DELIMETER
                 so the tokenmatcher will recognize it*/

                itrChar = PeekNextChar(input);

                while (itrChar.Value != STRING_DELIMETER)
                {
                    Advance();
                }

                sBuilder.Append(itrChar.Value);
                index++;
            }
            else if (itrChar.Value == EQUALSIGN)
            {
                itrChar = PeekNextChar(input);

                if (itrChar.Value == EQUALSIGN || itrChar.Value == ARROWRIGHT)
                {
                    sBuilder.Append(itrChar.Value);
                    index++;
                }
            }
            else if (itrChar.Value == ARROWLEFT || itrChar.Value == ARROWRIGHT)
            {
                itrChar = PeekNextChar(input);

                if (itrChar.Value == EQUALSIGN)
                {
                    sBuilder.Append(itrChar.Value);
                    index++;
                }
            }

            void Advance()
            {
                sBuilder.Append(itrChar.Value);
                index++;
                itrChar = PeekNextChar(sourceString);
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Creates a token from a char
        /// </summary>
        /// <param name="input"></param>
        private Token Create(char? input)
        {
            if (input == null)
                return EndCreateNewToken(TokenType.EOF, null);

            return Create(input.ToString());
        }

        /// <summary>
        /// Creates a token from a string 
        /// </summary>
        /// <param name="input"></param>
        private Token Create(string input)
        {
            if (IsKeyword(input))
            {
                return EndCreateKeywordToken(input);
            }

            foreach (TokenMatcher matcher in tokenMatchers)
            {
                if (matcher.IsMatch(input))
                {
                    if (matcher.Type == TokenType.ID)
                    {
                        char? next = PeekNextChar(sourceString);
                        if (next.HasValue)
                        {
                            if (next.Value == '(')
                            {
                                return EndCreateNewToken (TokenType.CALL, matcher.MatchValue);
                            }
                        }
                    }
                    return EndCreateNewToken(matcher.Type, matcher.MatchValue);
                }
            }
            return EndCreateNewToken(TokenType.NONE, null);
        }
    }
}
