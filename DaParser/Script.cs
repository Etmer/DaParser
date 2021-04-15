using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    
    public class Script<T> where T : Interpreter, new()
    {
        public T Interpreter { get; private set; }

        public ScriptErrorHandler ErrorHandler;
        private Lexer lexer;
        private Parser parser;
        private SemanticAnalyzer analyzer;

        public void Parse(string input) 
        {
            ErrorHandler = new ScriptErrorHandler();

            lexer = new Lexer();
            parser = new Parser();
            analyzer = new SemanticAnalyzer();

            lexer.OnError += ErrorHandler.RaiseError;
            analyzer.OnError += ErrorHandler.RaiseError;
            parser.OnError += ErrorHandler.RaiseError;

            lexer.Tokenize(input);

            if (!ErrorHandler.HasErrors)
            {
                Node node = parser.Parse(lexer);
                SymbolTable table = analyzer.Analyze(node);

                if (!ErrorHandler.HasErrors)
                {
                    Interpreter = new T();
                    Interpreter.SetSymbolTable(table);
                    Interpreter.SetTree(node);
                }
            }
        }
    }
}
