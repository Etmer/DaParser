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
                function
                    MyVar = 5;
                    if(MyVar == 9) then
                        MySecondVar = MyVar + 4;
                    else
                        MySecondVar = MyVar + 6;
                    end
                end
            end
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
            Interpreter interpreter = new Interpreter(parser.Parse());
            interpreter.Interpret();
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
