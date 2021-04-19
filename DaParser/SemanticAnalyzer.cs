using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class SemanticAnalyzer : ErrorRaiser
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
                case TokenType.ID:
                    Visit_Id(node);
                    break;
            }
        }
        private void Visit_VariableDeclaration(Node node)
        {
            VariableDeclarationNode varDeclNode = node as VariableDeclarationNode;

            string type = (string)node.Token.Value;

            if (table.LookUp(type, out ISymbol symbol))
            {
                string symbolName = varDeclNode.Variable.GetValue() as string;
                ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                if (!table.Define(varSymbol))
                    throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, varDeclNode.Variable.Token);

                return;
            }
        }

        private void Visit_Id(Node node)
        {
            string name = node.GetValue() as string;

            if (!table.LookUp(name, out ISymbol symbol))
            {
                throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, node.Token);
            }

        }

        private void Visit_AssignNode(Node node)
        {
            string name = node.children[0].GetValue() as string;

            Visit_Expression(node.children[1]);
        }

        private void Visit_Expression(Node node)
        {
            Token token = node.Token;

            switch (token.Type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.MUL:
                case TokenType.DIV:
                case TokenType.ID:
                    Visit_ExpressionPart(node.children[0]);
                    Visit_ExpressionPart(node.children[1]);
                    break;
            }
        }

        private void Visit_ExpressionPart(Node node)
        {
            Token token = node.Token;

            switch (token.Type)
            {
                case TokenType.ID:
                    Visit_Id(node);
                    break;
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.DIV:
                case TokenType.MUL:
                    Visit_Expression(node);
                    break;
            }
        }
    }
}
