using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public enum LexicalError 
    {

    }

    public class Lexer
    {
        private StringBuilder sBuilder;

        private Token currentToken;
        private int currentLine = 1;
        private int currentColumn = 0;

        private string input;
        private char? currentChar;
        private int index = 0;
        private List<Token> tokens = new List<Token>();
        private List<TokenMatcher> tokenMatchers = new List<TokenMatcher>()
        {
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
            { new TokenMatcher(TokenType.STRING, @"(?<=\')(.?)*(?=\')")},
            { new TokenMatcher(TokenType.EOF, @"(\0)")},
            { new TokenMatcher(TokenType.ID, @"\b((?i)[a-aZ-z_][a-aZ-z0-9_]*)\b")},
            { new TokenMatcher(TokenType.L_PAREN, @"(\()")},
            { new TokenMatcher(TokenType.NUMBER, @"\d+")},
            { new TokenMatcher(TokenType.R_PAREN, @"(\))")},
            { new TokenMatcher(TokenType.EOF, @"(\!)")},
        };

        private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "if", TokenType.CONDITION },
            { "else", TokenType.ELSE },
            { "elif", TokenType.ELSEIF },
            { "program", TokenType.PROGRAM },
            { "end",  TokenType.END },
            { "then",  TokenType.THEN },
            { "string", TokenType.TYPESPEC },
            { "int", TokenType.TYPESPEC },
            { "double", TokenType.TYPESPEC },
            { "bool", TokenType.TYPESPEC },
        };

        public bool HasToken() 
        {
            return index < tokens.Count;
        }

        public void Tokenize(string input)
        {
            this.input = input;
            currentChar = GetNextChar();

            sBuilder = new StringBuilder();

            while(currentChar != null)
            {
                switch (currentChar)
                {
                    case '\r':
                        break;
                    case '\n':
                        currentLine++;
                        currentColumn = 0;
                        break;
                    case ' ':
                        break;
                    case '(':
                    case ')':
                        StartCreateNewToken(currentLine,currentColumn);
                        CreateTokenFromChar(currentChar.Value);
                        break;

                    case '[':
                    case ']':
                        StartCreateNewToken(currentLine, currentColumn);
                        CreateTokenFromChar(currentChar.Value);
                        break;
                    case '\'':
                        StartCreateNewToken(currentLine, currentColumn);
                        HandleString(input, sBuilder);
                        break;
                    case '=':
                    case '-':
                    case '+':
                        StartCreateNewToken(currentLine, currentColumn);
                        char? next = PeekNextChar();
                        if (next.HasValue)
                        {
                            if (next == '=')
                            {
                                sBuilder.Append(currentChar);
                                sBuilder.Append(next);
                                Do(sBuilder.ToString());
                                currentChar = GetNextChar();
                                currentChar = GetNextChar();
                                break;
                            }
                        }
                        Do(currentChar.ToString());
                        break;
                    case ';':
                        StartCreateNewToken(currentLine, currentColumn);
                        Do(currentChar.ToString());
                        break;
                    case '!':
                        StartCreateNewToken(currentLine, currentColumn);
                        CreateTokenFromChar(currentChar.Value);
                        break;
                    default:
                        StartCreateNewToken(currentLine, currentColumn);
                        CreateIDToken(sBuilder);
                        Do(sBuilder.ToString());
                        break;
                }

                currentChar = GetNextChar();
            }
            index = 0;
        }

        public Token GetNextToken() 
        {
            return tokens[index++];
        }

        private void Do(string input) 
        {
            if (IsKeyword(input)) 
            {
                tokens.Add(CreateTokenForKeyword(input));
                sBuilder.Clear();
                return;
            }

            foreach (TokenMatcher matcher in tokenMatchers)
            {
                if (matcher.IsMatch(input))
                {
                    if (matcher.Type == TokenType.ID)
                    {
                        char? next = PeekNextChar();
                        if (next.HasValue)
                        {
                            if(next.Value == '(')
                            {
                                EndCreateNewToken(TokenType.CALL, input);
                                break;
                            } 
                        }
                    }
                    EndCreateNewToken(matcher.Type, input);
                    break;
                }
            }
        }

        private char? GetNextChar()
        {
            index++;
            currentColumn++;

            if (index < input.Length)
            {
                return input[index];
            }
            return null;
        }

        private char? PeekNextChar()
        {
            int idx = index + 1;
            if (idx < input.Length)
                return input[idx];

            return null;
        }

        private void CreateTokenFromStringBuilder(StringBuilder stringBuilder) 
        {
            if (stringBuilder.Length != 0)
            {
                Do(stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }

        private void CreateTokenFromChar(char character)
        {
            Do(character.ToString());
        }

        private void HandleString(string input, StringBuilder sBuilder) 
        {
            CreateTokenFromStringBuilder(sBuilder);

            sBuilder.Append(currentChar);
            currentChar = GetNextChar().Value;

            while (currentChar != '\'')
            {
                sBuilder.Append(input[index]);
                currentChar = GetNextChar().Value;
            }
            sBuilder.Append(currentChar);

            CreateTokenFromStringBuilder(sBuilder);
        }

        private bool IsKeyword(string input) 
        {
            return keywords.ContainsKey(input);
        }

        private Token CreateTokenForKeyword(string input)
        {
            TokenType type = keywords[input];
            currentToken.SetType(type);
            currentToken.SetValue(input);
            return currentToken;
        }


        public void StartCreateNewToken(int line, int column) 
        {
            currentToken = new Token();
            currentToken.SetPosition(line, column);
        }

        public void EndCreateNewToken(TokenType type, string input) 
        {
            currentToken.SetType(type);
            currentToken.SetValue(input);
            tokens.Add(currentToken);
            sBuilder.Clear();

        }

        private void CreateIDToken(StringBuilder sBuilder) 
        {
            char? itrChar = currentChar;
            sBuilder.Append(itrChar.Value);

            if (char.IsDigit(itrChar.Value))
            {
                itrChar = PeekNextChar();
                while (char.IsDigit(itrChar.Value))
                {
                    sBuilder.Append(itrChar.Value);
                    index++;
                    itrChar = PeekNextChar();
                }
            }
            else if (char.IsLetter(itrChar.Value))
            {
                itrChar = PeekNextChar();
                while (char.IsLetterOrDigit(itrChar.Value))
                {
                    sBuilder.Append(itrChar.Value);
                    index++;
                    itrChar = PeekNextChar();
                }
            }
        }
    }
}
