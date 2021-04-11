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
                case TokenType.TYPESPEC:
                    Visit_VariableDeclaration(node);
                    break;
                case TokenType.L_BLOCK:
                    CompundStatementNode compundNode = node as CompundStatementNode;

                    foreach (Node statement in compundNode.statementList)
                        Visit(statement);
                    break;
                case TokenType.ASSIGN:
                    Visit_AssignNode(node);
                    break;
            }
        }

        private void VisitBlock(Node node) 
        {

        }

        private void Visit_VariableDeclaration(Node node) 
        {
            VariableDeclarationNode varDeclNode = node as VariableDeclarationNode;

            string type = (string)node.Token.Value;

            if (table.LookUp(type, out ISymbol symbol))
            {
                string symbolName = varDeclNode.Variable.GetValue() as string;
                ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                table.Define(varSymbol);
                return;
            }
            throw new System.Exception();
        }

        private void Visit_Id(Node node) 
        {
            string name = node.GetValue() as string;

            if (!table.LookUp(name, out ISymbol symbol))
            {
                throw new System.Exception($"Symbol with name <{name}> is undefined");
            }

        }

        private void Visit_AssignNode(Node node) 
        {
            string name = node.children[0].GetValue() as string;

            if (!table.LookUp(name, out ISymbol symbol))
            {
                throw new System.Exception($"Symbol with name <{name}> is undefined");
            }
        }
    }
}
