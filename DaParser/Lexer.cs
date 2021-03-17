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
            { new TokenMatcher(TokenType.CONDITION, @"\b(if)\b")},
            { new TokenMatcher(TokenType.FUNC, @"\b(function)\b")},
            { new TokenMatcher(TokenType.EOF, @"(\!)")},

            { new TokenMatcher(TokenType.L_BLOCK, @"(\[)")},
            { new TokenMatcher(TokenType.R_BLOCK, @"(\])")},
            { new TokenMatcher(TokenType.COMMA, @"(\,)")},

            { new TokenMatcher(TokenType.END, @"\b(end)\b")},
            { new TokenMatcher(TokenType.PROGRAM, @"\b(program)\b")},
            { new TokenMatcher(TokenType.EQUALS, @"(\==)")},
            { new TokenMatcher(TokenType.ASSIGN, @"(\=)")},
            { new TokenMatcher(TokenType.PLUS, @"(\+)")},
            { new TokenMatcher(TokenType.SEMI, @"(\;)")},
            { new TokenMatcher(TokenType.MINUS, @"(\-)")},
            { new TokenMatcher(TokenType.MUL, @"(\*)")},
            { new TokenMatcher(TokenType.DIV, @"(\/)")},
            { new TokenMatcher(TokenType.STRING, @"(?<=\')(.?)*(?=\')")},
            { new TokenMatcher(TokenType.EOF, @"(\0)")},
            { new TokenMatcher(TokenType.THEN,@"\b(then)\b")},
            { new TokenMatcher(TokenType.ELSEIF,@"\b(elif)\b")},
            { new TokenMatcher(TokenType.ELSE,@"\b(else)\b")},
            { new TokenMatcher(TokenType.ID, @"\b((?i)[a-aZ-z_][a-aZ-z0-9_]*)\b")},
            { new TokenMatcher(TokenType.L_PAREN, @"(\()")},
            { new TokenMatcher(TokenType.NUMBER, @"\d+")},
            { new TokenMatcher(TokenType.R_PAREN, @"(\))")},
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
                        char next = ' ';
                        if (PeekNextChar(ref next, input))
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
            foreach (TokenMatcher matcher in tokenMatchers)
            {
                Token token = Token.CreateEmpty();
                if (matcher.IsMatch(input, ref token))
                {
                    tokens.Add(token);
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

        private bool PeekNextChar(ref char next, string input)
        {
            int idx = index + 1;
            if (index < input.Length)
            {
                next = input[idx];
                return true;
            }
            return false;
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
    }
}
