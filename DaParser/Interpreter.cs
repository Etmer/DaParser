using EventScript.Interfaces;
using EventScript.Literals;
using System;
using System.Collections.Generic;

namespace EventScript
{
    public class Interpreter : ErrorRaiser, IVisitor
    {
        public Environment GlobalMemory { get; private set; } = new Environment();
        private Code code = null;
        private SymbolTable symbolTable;

        public void SetCode(Code code) { this.code = code; }
        public void Interpret()
        {
            code.Accept(this);
        }

        public object Visit_Program(Code code)
        {
            return code.BlockStatement.Accept(this);   
        }

        public object Visit_ConditionalExpression(ConditionalExpression expr)
        {
            foreach (ConditionBlock block in expr.conditionBlocks)
            {
                if ((bool)block.Condition.Accept(this)) 
                {
                    return block.BlockStatement.Accept(this);
                }
            }
            return 0;
        }

        public object Visit_BinaryExpression(BinaryExpression expr)
        {
            switch (expr.OperatorType) 
            {
                case TokenType.PLUS:
                    return (double)expr.Left.Accept(this) + (double)expr.Right.Accept(this);
                case TokenType.MINUS:
                    return (double)expr.Left.Accept(this) - (double)expr.Right.Accept(this);
                case TokenType.MUL:
                    return (double)expr.Left.Accept(this) * (double)expr.Right.Accept(this);
                case TokenType.DIV:
                    return (double)expr.Left.Accept(this) / (double)expr.Right.Accept(this);
            }
            return 0;
        }

        public object Visit_UnaryExpression(UnaryExpression expr)
        {
            switch (expr.OperatorType)
            {
                case TokenType.PLUS:
                    return   (double)expr.Expression.Accept(this);
                case TokenType.MINUS:
                    return  -(double)expr.Expression.Accept(this);
            }
            return 0;
        }

        public object Visit_BlockStatement(BlockStatement block)
        {
            foreach (IExpression stmt in block.Satements)
                stmt.Accept(this);

            return 0;
        }

        public object Visit_BlockVariable(BlockVariable blockVar)
        {
            string varName = (string)blockVar.Name.Accept(this);

            GlobalMemory[varName] = blockVar.BlockStatement;

            return 0;
        }

        public object Visit_DeclarationStatement(DeclarationStatement declStmt)
        {
            string varName = (string)declStmt.DeclaredVariable.Accept(this);
            GlobalMemory[varName] = declStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_AssignStatement(AssignStatement assignStmt)
        {
            string varName = (string)assignStmt.Variable.Accept(this);
            GlobalMemory[varName] = assignStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_Variable(Variable variable)
        {
            return GlobalMemory[variable.Name];
        }

        public object Visit_FunctionExpression(FunctionCallExpression func)
        {
            throw new NotImplementedException();
        }

        public object Visit_ConditionBlock(ConditionBlock condBlock)
        {
            throw new NotImplementedException();
        }

        public object Visit_DialogueExpression(DialogueExpression dialogueExpr)
        {
            throw new NotImplementedException();
        }

        public object Visit_TextMemberExpression(TextMemberExpression txtMember)
        {
            throw new NotImplementedException();
        }

        public object Visit_ChoiceMemberExpression(ChoiceMemberExpression choiceMember)
        {
            throw new NotImplementedException();
        }
    }
}
