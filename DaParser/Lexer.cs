﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class Lexer
    {
        private int index = 0;
        private List<Token> tokens = new List<Token>();
        private List<TokenMatcher> tokenMatchers = new List<TokenMatcher>()
        {
            { new TokenMatcher(TokenType.CONDITION, @"\b(if)\b")},
            { new TokenMatcher(TokenType.EQUALS, @"(\=)")},
            { new TokenMatcher(TokenType.PLUS, @"(\+)")},
            { new TokenMatcher(TokenType.SEMI, @"(\;)")},
            { new TokenMatcher(TokenType.MINUS, @"(\-)")},
            { new TokenMatcher(TokenType.MUL, @"(\*)")},
            { new TokenMatcher(TokenType.DIV, @"(\/)")},
            { new TokenMatcher(TokenType.STRING, @"\'(.?)*\'")},
            { new TokenMatcher(TokenType.EOF, @"(\0)")},
            { new TokenMatcher(TokenType.ID, @"\b([a-aZ-z_][a-aZ-z0-9_]*)\b")},
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
            StringBuilder sBuilder = new StringBuilder();

            while(index < input.Length)
            {
                char current = input[index];
                switch (current)
                {
                    case '\r':
                    case '\n':
                        Do(sBuilder.ToString());
                        sBuilder.Clear();

                        GetNextChar(ref current, input);
                        break;
                    case ' ':
                        if (sBuilder.Length > 0)
                        {
                            Do(sBuilder.ToString());
                            sBuilder.Clear();
                        }

                        GetNextChar(ref current, input);
                        break;
                    case '(':
                    case ')':
                        Do(sBuilder.ToString());
                        Do(current.ToString());
                        sBuilder.Clear();

                        GetNextChar(ref current, input);
                        break;
                    case '\'':
                        sBuilder.Append(current);
                        GetNextChar(ref current, input);

                        while (current!= '\'')
                        {
                            sBuilder.Append(input[index]);
                            GetNextChar(ref current, input);
                        }
                        sBuilder.Append(current);
                        Do(sBuilder.ToString());
                        sBuilder.Clear();
                        GetNextChar(ref current, input);
                        break;
                    case '=':
                    case '-':
                    case '+':
                        Do(current.ToString());
                        GetNextChar(ref current, input);
                        break;
                    default:
                        sBuilder.Append(input[index]);
                        index++;
                        break;
                }
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

        private bool GetNextChar(ref char current, string input)
        {
            index++;
            if (index < input.Length)
            {
                current = input[index];
                return true;
            }
            return false;
        }
    }
}