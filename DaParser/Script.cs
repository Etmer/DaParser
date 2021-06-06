using EventScript.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventScript
{
    public class Script : Interpreter
    {
        public Interpreter Interpreter { get; private set; }

        private Lexer lexer;
        private Parser parser;
        private SemanticAnalyzer analyzer;

        public Script()
        {
            lexer = new Lexer();
            parser = new Parser();
            analyzer = new SemanticAnalyzer();
            Interpreter = new Interpreter();

        }
        public void Parse(string input)
        {
            lexer.Tokenize(input);
            Code code = parser.Parse(lexer);
            analyzer.SetCode(code);
            analyzer.PreVisit();

            Interpreter.SetCode(code);
        }
    }

    public class BehaviourScript : Script { }

    public class DialogueScript: Script
    {
      
    }
}
