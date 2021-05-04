using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class SemanticAnalyzer : ErrorRaiser
    {
        private SymbolTable currentTable;
        private bool hasEntryPoint = false;
        private const string ENTRYBLOCKNAME = "Start";
        public SymbolTable Analyze(Node node)
        {
            currentTable = new SymbolTable();

            CompundStatementNode compundNode = node as CompundStatementNode;

            foreach (Node statement in compundNode.statementList)
                Visit(statement);

            if (!hasEntryPoint)
                throw RaiseError(ScriptErrorCode.UNDEFINDED_ENTRYPOINT,node.Token);

            return currentTable;
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
                    Visit_BlockNode(node);
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
            Node assignment = node.children[0];
            Node variable = assignment.children[0];

            string type = (string)node.Token.Value;

            if (currentTable.LookUp(type, out ISymbol symbol))
            {
                string symbolName = variable.GetValue() as string;
                ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                if (!currentTable.Define(varSymbol))
                    throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, variable.Token);

                return;
            }
            throw RaiseError(ScriptErrorCode.UNDEFINED_SYMBOL, variable.Token);
        }

        private void Visit_Id(Node node)
        {
            string name = node.GetValue() as string;

            if (!currentTable.LookUp(name, out ISymbol symbol))
                throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, node.Token);
        }

        private void Visit_BlockNode(Node node) 
        {
            CompundStatementNode compundNode = node as CompundStatementNode;

            SymbolTable table = new SymbolTable(currentTable);
            string blockName = (string)node.children[0].GetValue();

            if (blockName == ENTRYBLOCKNAME)
                hasEntryPoint = true;

            table.DefineDialogueSymbols();
            currentTable = table;
         
            foreach (Node statement in compundNode.statementList)
                Visit_StatementNode(statement);

            currentTable = currentTable.ParentTable;
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
        private void Visit_StatementNode(Node node)
        {
            Token token = node.Token;

            switch (token.Type) 
            {
                case TokenType.TEXT_MEMBER:
                    Visit_TexMemberNode(node);
                    break;
            }
        }

        private void Visit_TexMemberNode(Node node)
        {
            string name = "Dialogue";

            if (currentTable.LookUp(name, out ISymbol symbol))
            {
                string symbolName = "New_ Dialogue";
                ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                if (!currentTable.Define(varSymbol))
                    throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, node.Token);
            }
        }
    }
}
