using EventScript.Interfaces;
using EventScript.Literals;
using System;
using System.Collections.Generic;

namespace EventScript
{
    public partial class Interpreter : ErrorRaiser, IVisitor
    {
        public Environment Current { get; private set; } = new Environment();

        private Environment last = null;
        private Code code = null;

        public void SetCode(Code code) { this.code = code; }
        public virtual void PreVisit()
        {
            code.Accept(this);
        }

        public object EnterBlockNode(string input) 
        {
            last = Current;
            Current = new Environment(last);

            Current["BlockData"] = new DialogueData();
            BlockValue stmt = Current[input] as BlockValue;

            return Visit_BlockStatement(stmt.GetValue() as BlockStatement);
        }

        public object Visit_Program(Code code)
        {
            return code.BlockStatement.Accept(this);   
        }

        public object Visit_ConditionalExpression(ConditionalExpression expr)
        {
            if ((bool)expr.IfBlock.Accept(this))
                expr.IfBlock.BlockStatement.Accept(this);
            else if(expr.ElseBlock != null)
                expr.ElseBlock.Accept(this);

            return 0;
        }

        public object Visit_BinaryExpression(BinaryExpression expr)
        {
            IComparable lhs;
            IComparable rhs;

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
               
                case TokenType.EQUALS:
                    lhs = (IComparable)expr.Left.Accept(this);
                    rhs = (IComparable)expr.Right.Accept(this);

                    return lhs.CompareTo(rhs) == 0;

                case TokenType.SMALLER:
                    lhs = (IComparable)expr.Left.Accept(this);
                    rhs = (IComparable)expr.Right.Accept(this);

                    return lhs.CompareTo(rhs) == -1;

                case TokenType.GREATER:
                    lhs = (IComparable)expr.Left.Accept(this);
                    rhs = (IComparable)expr.Right.Accept(this);

                    return lhs.CompareTo(rhs) == 1;

                case TokenType.SMALLEREQUALS:
                    lhs = (IComparable)expr.Left.Accept(this);
                    rhs = (IComparable)expr.Right.Accept(this);

                    return lhs.CompareTo(rhs) == -1;

                case TokenType.GREATEREQUALS:
                    lhs = (IComparable)expr.Left.Accept(this);
                    rhs = (IComparable)expr.Right.Accept(this);

                    return lhs.CompareTo(rhs) == 1;

                case TokenType.CONDITION:
                    if ((bool)expr.Left.Accept(this))
                        expr.Right.Accept(this);
                    break;
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
            object obj = null;

            foreach (IExpression stmt in block.Satements)
                obj = stmt.Accept(this);

            return obj;
        }

        public object Visit_DeclarationStatement(DeclarationStatement declStmt)
        {
            string varName = declStmt.Name;
            Current[varName] = declStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_AssignStatement(AssignStatement assignStmt)
        {
            string varName = (string)assignStmt.Variable.Name;
            Current[varName] = assignStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_Variable(Variable variable)
        {
            return ((IValue)Current[variable.Name]).GetValue();
        }

        public object Visit_FunctionCallExpression(FunctionCallExpression func)
        {
            string name = func.Callee;

            List<object> arguments = new List<object>();

            foreach (IExpression argument in func.Arguments)
            {
                object value = argument.Accept(this);
                arguments.Add(value);
            }

            FunctionValue funcValue = (FunctionValue)Current[name];


            return funcValue.GetValue(arguments);
        }

        public object Visit_ConditionBlock(ConditionBlock condBlock)
        {
            return condBlock.Condition.Accept(this);
        }

        public string Visit_StringLiteral(StringLiteral lit)
        {
            return lit.GetValue<string>();
        }

        public double Visit_NumberLiteral(NumberLiteral lit)
        {
            return lit.GetValue<double>();
        }

        public bool Visit_BooleanLiteral(BooleanLiteral lit)
        {
            return lit.GetValue<bool>();
        }

        public object Visit_EndExpression(EndExpression endExpr)
        {
            return 0;
        }

        public object Visit_BlockDeclarationStatement(BlockDeclarationExpression declExpr)
        {
            string varName = declExpr.Name;
            Current[varName] = new BlockValue(declExpr.Expression);

            return 0;
        }
    }
}
