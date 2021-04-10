using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Script
    {
        private Lexer lexer;
        private Parser parser;
        private SemanticAnalyzer analyzer;

        public Node Parse(string input) 
        {
            lexer = new Lexer();
            analyzer = new SemanticAnalyzer();

            lexer.Tokenize(input);
            parser = new Parser(lexer);

            Node node = parser.Parse();

            analyzer.Analyze(node);
            
            return node;
        }
    }
}
