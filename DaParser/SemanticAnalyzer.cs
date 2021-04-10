using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class SemanticAnalyzer
    {
        private SymbolTable table;
        public SymbolTable Analyze(Node node)
        {
            table = new SymbolTable();

            CompundStatementNode compundNode = node as CompundStatementNode;

            foreach (Node statement in compundNode.statementList)
                Visit(statement);

            return table;
        }

        private void Visit(Node node) 
        {
            Token token = node.Token;

            switch (token.Type) 
            {
                case TokenType.ASSIGN:
                    VisitVariableDeclaration(node);
                    break;
            }
        }

        private void VisitBlock(Node node) 
        {

        }

        private void VisitVariableDeclaration(Node node) 
        {

        }
    }
}
