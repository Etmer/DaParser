using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Script<T> where T : Interpreter, new()
    {
        public T Interpreter { get; private set; }

        private Lexer lexer;
        private Parser parser;
        private SemanticAnalyzer analyzer;

        public void Parse(string input) 
        {
            lexer = new Lexer();
            parser = new Parser();
            analyzer = new SemanticAnalyzer();
            lexer.Tokenize(input);

            Node node = parser.Parse(lexer);
            SymbolTable table = analyzer.Analyze(node);

            Interpreter = new T();
            Interpreter.SetSymbolTable(table);
            Interpreter.SetTree(node);
        }
    }
}
