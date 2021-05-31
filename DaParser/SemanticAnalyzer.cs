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

        public object Visit_DialogueChoiceExpression(DialogueChoiceExpression choiceMember)
    {
            string name = choiceMember.Next.Accept(this) as string;

            if (!currentTable.LookUp(name, out ISymbol symbol))
                throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, ((NodeBase)choiceMember.Next).Token);
           
            return 0;
        }

        public object Visit_ConditionalExpression(ConditionalExpression expr)
        {
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

            foreach (IDialogueMember expr in dialogueExpr.MemberList)
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

        public object Visit_DialogueTextExpression(DialogueTextExpression txtMember)
        {
            if (txtMember.Next != null)
            {
                string name = txtMember.Next.Accept(this) as string;

                if (!currentTable.LookUp(name, out ISymbol symbol))
                    throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, ((NodeBase)txtMember.Next).Token);
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

        public object Visit_DialogueActorExpression(DialogueActorExpression actorExpr)
        {
            return 0;
        }

        public object Visit_DialogueMoodExpression(DialogueMoodExpression moodExpr)
        {
            return 0;
        }
    }
}