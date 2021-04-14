using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Script<T> where T : Interpreter, new()
    {
        private Lexer lexer;
        private Parser parser;
        private SemanticAnalyzer analyzer;
        public T Interpreter { get; private set; }

        public void Parse(string input) 
        {
            lexer = new Lexer();
            analyzer = new SemanticAnalyzer();

            lexer.Tokenize(input);
            parser = new Parser(lexer);

            Node node = parser.Parse();
            SymbolTable table = analyzer.Analyze(node);

            Interpreter = new T();
            Interpreter.SetSymbolTable(table);
            Interpreter.SetTree(node);
        }
    }
}
