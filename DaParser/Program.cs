﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DaScript
{
    class Program
    {
        static string s =
            @"
            program
               
                string s = 'TestString';
                int i = 1;
                double d = 1.2;
                bool b = GetMeMyBool();

                [Start]
                    SetText('Hello Adventurer');
                    SetChoice('Show me your wares!', 'Wares');

                    if(i == 1) then
                        SetChoice('I did not find this on the doorstep', 'Quest');
                    end

                    if(b) then
                        SetChoice('I found this on the doorstep', 'Quest');
                    end
                end

                [Wares]
                    SetText('I will not talk to you if you dont need anything');
                    SetChoice('I need Potions','MyNextName');
                    SetChoice('K Bye','Final');
                end  
                
                [Quest]
                    GoTo('MyNextName');
                end

                [MyNextName]
                    SetText('My treasured trousers!!! I am so relieved thank you');
                    SetChoice('I need Potions','Final');
                end  
                
                [Final]
                    SetText('Thank you!');
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
            DialogueInterpreter interpreter = new DialogueInterpreter(parser.Parse());
            interpreter.Interpret();

            interpreter.StartDialogue();

            while (true)
            {
                int idx = 0;
                if (int.TryParse(Console.ReadLine(), out idx))
                    interpreter.UpdateDialogue(idx); 
            }
        }
    }


    public class TokenMatcher 
    {
        public TokenType Type { get;private set; }
        private Regex pattern;
        private string matchValue;

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
                matchValue = match.Value;
            }
            return match.Success;
        }

        public Token CreateTokenFromMatch(TokenType? overwriteType = null) 
        {
            if(overwriteType.HasValue) 
                return new Token(overwriteType.Value, matchValue);

            return new Token(Type, matchValue);
        }

    }
}
