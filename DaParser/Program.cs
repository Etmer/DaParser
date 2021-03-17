using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DaScript
{
    class Program
    {
        delegate object Function(List<object> args);

        static string s =
            @"
            program

                [MyName]
                  CallMeMaybe();
                end

                [MyNextName]
                    AnotherVar = 'hello';
                end

            !
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
            EventInterpreter interpreter = new EventInterpreter(parser.Parse());
            interpreter.Interpret();
            string nextInput = Console.ReadLine();
            interpreter.VisitBlock(nextInput);

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
