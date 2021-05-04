using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
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
            Node node = parser.Parse(lexer);
            SymbolTable table = analyzer.Analyze(node);

            Interpreter.SetSymbolTable(table);
            Interpreter.SetTree(node);
        }
    }
}
