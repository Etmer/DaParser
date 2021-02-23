using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DaScript
{
    class Program
    {
        static string s =
            @"
            5 + 3 - 4 + 10 * 2
        ";

        static void Main(string[] args)
        {
            CreateTokens(s);
        }

        static void CreateTokens(string input)
        {
            Lexer lexer = new Lexer();
            lexer.Tokenize(input);
            Parser parser = new Parser(lexer);
            parser.Parse();
            Console.ReadKey();
        }
    }

    public class TokenMatcher 
    {
        private TokenType Type;
        private Regex pattern;

        public TokenMatcher(TokenType type, string patternString) 
        {
            Type = type;
            pattern = new Regex(patternString);
        }

        public bool IsMatch(string input, ref Token token) 
        {
            Match match = pattern.Match(input);

            if (match.Success)
            {
                token.Set(Type, match.Value);
                return true;
            }
            return false;
        }

    }
}
