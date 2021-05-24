using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript
{
    class SemanticAnalyzer : ErrorRaiser, IVisitor
    {
        private const string ENTRYBLOCKNAME = "Start";
        private bool hasEntryPoint = false;

        private SymbolTable currentTable;
        private Code code = null;

        public void SetCode(Code code) { this.code = code; }

        public void Visit()
        {
            currentTable = new SymbolTable();

            code.Accept(this);
        }

        public object Visit_AssignStatement(AssignStatement assignStmt)
        {
            assignStmt.Variable.Accept(this);
            assignStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_BinaryExpression(BinaryExpression expr)
        {
            expr.Left.Accept(this);
            expr.Right.Accept(this);

            return 0;
        }

        public object Visit_BlockStatement(BlockStatement block)
        {
            foreach (IExpression expr in block.Satements)
                expr.Accept(this);

            return 0;
        }

        public object Visit_BlockVariable(BlockVariable blockVar)
        {
            return blockVar.BlockStatement.Accept(this);
        }
        public bool Visit_BooleanLiteral(BooleanLiteral lit)
        {
            Visit_Literal(lit);
            return lit.GetValue<bool>();
        }

        public object Visit_ChoiceMemberExpression(ChoiceMemberExpression choiceMember)
    {
            string name = choiceMember.Next.Accept(this) as string;

            if (!currentTable.LookUp(name, out ISymbol symbol))
                throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, choiceMember.Token);
           
            return 0;
        }

        public object Visit_ConditionalExpression(ConditionalExpression expr)
        {
            int currentHighestPrecedence = 0;

            foreach (ConditionBlock block in expr.conditionBlocks) 
            {
                if (block.Precedence >= currentHighestPrecedence)
                {
                    block.Accept(this);
                    currentHighestPrecedence = block.Precedence;
                }
                else 
                    throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, block.Token);
            }
            return 0;
        }

        public object Visit_ConditionBlock(ConditionBlock condBlock)
        {
            return condBlock.BlockStatement.Accept(this);
        }

        public object Visit_DeclarationStatement(DeclarationStatement declStmt)
        {
            string type = declStmt.Type;

            if (currentTable.LookUp(type, out ISymbol symbol))
            {
                string symbolName = declStmt.Name;
                ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                if (!currentTable.Define(varSymbol))
                    throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, declStmt.Token);

                return 0;
            }
            throw RaiseError(ScriptErrorCode.UNDEFINED_SYMBOL, declStmt.Token);
        }

        public object Visit_DialogueExpression(DialogueExpression dialogueExpr)
        {
            dialogueExpr.TextExpression.Accept(this);

            foreach (ChoiceMemberExpression expr in dialogueExpr.ChoiceExpressionList)
                expr.Accept(this);

            return 0;
        }

        public object Visit_FunctionCallExpression(FunctionCallExpression func)
        {
            //Functions are added from outside of the environment
            //Whether or not they are already declared depends on the C# Script
            return 0;
        }

        public double Visit_NumberLiteral(NumberLiteral lit)
        {
            Visit_Literal(lit);
            return lit.GetValue<double>();
        }

        public object Visit_Program(Code code)
        {
            List<IExpression> postChecks = new List<IExpression>();

            foreach (IExpression expr in code.BlockStatement.Satements) 
            {
                switch (expr) 
                {
                    case BlockVariable blockVar:
                        if (currentTable.LookUp("Dialogue", out ISymbol symbol))
                        {
                            string symbolName = (string)blockVar.Name.Accept(this);
                            ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

                            if (!currentTable.Define(varSymbol))
                                throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, blockVar.Token);

                            postChecks.Add(blockVar); 
                        }

                        break;
                    case DeclarationStatement declStmt:
                        declStmt.Accept(this);
                        break;
                }
            }

            foreach (IExpression expr in postChecks)
                expr.Accept(this);

            return 0;
        }

        public string Visit_StringLiteral(StringLiteral lit)
        {
            Visit_Literal(lit);
            return lit.GetValue<string>();
        }

        public object Visit_TextMemberExpression(TextMemberExpression txtMember)
        {
            if (txtMember.Next != null)
            {
                string name = txtMember.Next.Accept(this) as string;

                if (!currentTable.LookUp(name, out ISymbol symbol))
                    throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, txtMember.Token);
            }
            return 0;
        }

        public object Visit_UnaryExpression(UnaryExpression expr)
        {
            return expr.Expression.Accept(this);
        }

        public object Visit_Variable(Variable variable)
        {
            string name = variable.Name;

            if (!currentTable.LookUp(name, out ISymbol symbol))
                throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, variable.Token);

            return 0;
        }

        private void Visit_Literal(Literal lit) 
        {
            ISymbol symbol;

            if (!currentTable.LookUp(lit.Type, out symbol))
                throw RaiseError(ScriptErrorCode.UNDEFINED_SYMBOL, lit.Token);
        }
    }
}
//    //private SymbolTable currentTable;
//    //private bool hasEntryPoint = false;
//    //private const string ENTRYBLOCKNAME = "Start";
//    //public SymbolTable Analyze(Node node)
//    //{
//    //    currentTable = new SymbolTable();

//    //    CompundStatementNode compundNode = node as CompundStatementNode;

//    //    foreach (Node statement in compundNode.statementList)
//    //        Visit(statement);

//    //    if (!hasEntryPoint)
//    //        throw RaiseError(ScriptErrorCode.UNDEFINDED_ENTRYPOINT,node.Token);

//    //    return currentTable;
//    //}

//    //private void Visit(Node node)
//    //{
//    //    Token token = node.Token;

//    //    switch (token.Type)
//    //    {
//    //        case TokenType.TYPESPEC:
//    //            Visit_VariableDeclaration(node);
//    //            break;
//    //        case TokenType.L_BLOCK:
//    //            Visit_BlockNode(node);
//    //            break;
//    //        case TokenType.ASSIGN:
//    //            Visit_AssignNode(node);
//    //            break;
//    //        case TokenType.ID:
//    //            Visit_Id(node);
//    //            break;
//    //    }
//    //}

//    //private void Visit_VariableDeclaration(Node node)
//    //{
//    //    Node assignment = node.children[0];
//    //    Node variable = assignment.children[0];

//    //    string type = (string)node.Token.Value;

//    //    if (currentTable.LookUp(type, out ISymbol symbol))
//    //    {
//    //        string symbolName = variable.GetValue() as string;
//    //        ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

//    //        if (!currentTable.Define(varSymbol))
//    //            throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, variable.Token);

//    //        return;
//    //    }
//    //    throw RaiseError(ScriptErrorCode.UNDEFINED_SYMBOL, variable.Token);
//    //}

//    //private void Visit_Id(Node node)
//    //{
//    //    string name = node.GetValue() as string;

//    //    if (!currentTable.LookUp(name, out ISymbol symbol))
//    //        throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, node.Token);
//    //}

//    //private void Visit_BlockNode(Node node) 
//    //{
//    //    CompundStatementNode compundNode = node as CompundStatementNode;

//    //    SymbolTable table = new SymbolTable(currentTable);
//    //    string blockName = (string)node.children[0].GetValue();

//    //    if (blockName == ENTRYBLOCKNAME)
//    //        hasEntryPoint = true;

//    //    table.DefineDialogueSymbols();
//    //    currentTable = table;

//    //    foreach (Node statement in compundNode.statementList)
//    //        Visit_StatementNode(statement);

//    //    currentTable = currentTable.ParentTable;
//    //}

//    //private void Visit_AssignNode(Node node)
//    //{
//    //    string name = node.children[0].GetValue() as string;

//    //    Visit_Expression(node.children[1]);
//    //}

//    //private void Visit_Expression(Node node)
//    //{
//    //    Token token = node.Token;

//    //    switch (token.Type)
//    //    {
//    //        case TokenType.PLUS:
//    //        case TokenType.MINUS:
//    //        case TokenType.MUL:
//    //        case TokenType.DIV:
//    //        case TokenType.ID:
//    //            Visit_ExpressionPart(node.children[0]);
//    //            Visit_ExpressionPart(node.children[1]);
//    //            break;
//    //    }
//    //}

//    //private void Visit_ExpressionPart(Node node)
//    //{
//    //    Token token = node.Token;

//    //    switch (token.Type)
//    //    {
//    //        case TokenType.ID:
//    //            Visit_Id(node);
//    //            break;
//    //        case TokenType.PLUS:
//    //        case TokenType.MINUS:
//    //        case TokenType.DIV:
//    //        case TokenType.MUL:
//    //            Visit_Expression(node);
//    //            break;
//    //    }
//    //}
//    //private void Visit_StatementNode(Node node)
//    //{
//    //    Token token = node.Token;

//    //    switch (token.Type) 
//    //    {
//    //        case TokenType.TEXT_MEMBER:
//    //            Visit_TexMemberNode(node);
//    //            break;
//    //    }
//    //}

//    //private void Visit_TexMemberNode(Node node)
//    //{
//    //    string name = "Dialogue";

//    //    if (currentTable.LookUp(name, out ISymbol symbol))
//    //    {
//    //        string symbolName = "New_ Dialogue";
//    //        ISymbol varSymbol = new VariableSymbol(symbolName, symbol);

//    //        if (!currentTable.Define(varSymbol))
//    //            throw RaiseError(ScriptErrorCode.ID_ALREADY_DECLARED, node.Token);
//    //    }
//    //}
//    //}
//}
