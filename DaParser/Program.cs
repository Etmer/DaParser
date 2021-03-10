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
                    stringythingy = 'Hi';
                    MyVar = 5;
                    if(MyVar == 6) then
                        MySecondVar = -4;      
                    elif(MyVar == 5) then
                        MySecondVar = -10;     
                    end
                end
            end!
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
