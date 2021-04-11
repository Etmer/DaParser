using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class Lexer
    {
        private string input;
        private char? current;
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

        private Dictionary<string, Token> keywords = new Dictionary<string, Token>()
        {
            { "if", new Token(TokenType.CONDITION, null) },
            { "else",  new Token(TokenType.ELSE, null) },
            { "elif", new Token(TokenType.ELSEIF, null) },
            { "program", new Token(TokenType.PROGRAM,null) },
            { "end",  new Token(TokenType.END,null)},
            { "then",  new Token(TokenType.THEN,null) },
            { "string",  new Token(TokenType.TYPESPEC, "String")},
            { "int",new Token(TokenType.TYPESPEC, "Integer")},
            { "double",new Token(TokenType.TYPESPEC, "Double")},
            { "bool", new Token(TokenType.TYPESPEC, "Boolean")},
        };

        public bool HasToken() 
        {
            return index < tokens.Count;
        }

        public void Tokenize(string input)
        {
            this.input = input;
            current = GetNextChar();

            StringBuilder sBuilder = new StringBuilder();

            while(current != null)
            {
                switch (current)
                {
                    case '\r':
                    case '\n':
                        CreateTokenFromStringBuilder(sBuilder);
                        break;
                    case ' ':
                        CreateTokenFromStringBuilder(sBuilder);
                        break;
                    case '(':
                    case ')':
                        CreateTokenFromStringBuilder(sBuilder);
                        CreateTokenFromChar(current.Value);
                        break;

                    case '[':
                    case ']':
                        CreateTokenFromStringBuilder(sBuilder);
                        CreateTokenFromChar(current.Value);
                        break;
                    case '\'':
                        HandleString(input, sBuilder);
                        break;
                    case '=':
                    case '-':
                    case '+':
                        CreateTokenFromStringBuilder(sBuilder);
                        char? next = PeekNextChar();
                        if (next.HasValue)
                        {
                            if (next == '=')
                            {
                                sBuilder.Append(current);
                                sBuilder.Append(next);
                                Do(sBuilder.ToString());
                                sBuilder.Clear();
                                current = GetNextChar();
                                current = GetNextChar();
                                break;
                            }
                        }
                        Do(current.ToString());
                        break;
                    case ';':
                        Do(sBuilder.ToString());
                        Do(current.ToString());
                        sBuilder.Clear();
                        break;
                    case '!':
                        CreateTokenFromStringBuilder(sBuilder);
                        CreateTokenFromChar(current.Value);
                        break;
                    default:
                        sBuilder.Append(current);
                        break;
                }

                current = GetNextChar();
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
                return;
            }

            foreach (TokenMatcher matcher in tokenMatchers)
            {
                if (matcher.IsMatch(input))
                {
                    if (matcher.Type == TokenType.ID)
                    {
                        if (current.HasValue)
                        {
                            if(current.Value == '(')
                            {
                                Token t = matcher.CreateTokenFromMatch(TokenType.CALL);
                                tokens.Add(t);
                                break;
                            }
                                
                        }

                    }
                    tokens.Add(matcher.CreateTokenFromMatch());
                    break;
                }
            }
        }

        private char? GetNextChar()
        {
            index++;
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

            sBuilder.Append(current);
            current = GetNextChar().Value;

            while (current != '\'')
            {
                sBuilder.Append(input[index]);
                current = GetNextChar().Value;
            }
            sBuilder.Append(current);

            CreateTokenFromStringBuilder(sBuilder);
        }

        private bool IsKeyword(string input) 
        {
            return keywords.ContainsKey(input);
        }

        private Token CreateTokenForKeyword(string input)
        {
            return keywords[input];
        }
    }
}
