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

        public void SetCode(Code code) { this.code = code; }
        public void Visit()
        {
            code.Accept(this);
        }

        public void EnterBlockNode(string input) 
        {
            BlockValue stmt = GlobalMemory[input] as BlockValue;

            Visit_BlockStatement(stmt.GetValue() as BlockStatement);
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
            string varName = declStmt.Name;
            GlobalMemory[varName] = declStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_AssignStatement(AssignStatement assignStmt)
        {
            string varName = (string)assignStmt.Variable.Name;
            GlobalMemory[varName] = assignStmt.Expression.Accept(this);

            return 0;
        }

        public object Visit_Variable(Variable variable)
        {
            return ((IValue)GlobalMemory[variable.Name]).GetValue();
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

            FunctionValue funcValue = (FunctionValue)GlobalMemory[name];


            return funcValue.GetValue(arguments);
        }

        public object Visit_ConditionBlock(ConditionBlock condBlock)
        {
            return condBlock.Accept(this);
        }

        public object Visit_DialogueExpression(DialogueExpression dialogueExpr)
        {
            dialogueExpr.TextExpression.Accept(this);

            foreach (ChoiceMemberExpression choice in dialogueExpr.ChoiceExpressionList)
            {
                if ((bool)choice.Condition.Accept(this))
                    choice.Accept(this);
            }

            return 0;
        }

        public object Visit_TextMemberExpression(TextMemberExpression txtMember)
        {
            Dialogue dialogue = GlobalMemory["Dialogue"] as Dialogue;

            dialogue.SetText((string)txtMember.Text.Accept(this));

            if (txtMember.Next != null) { dialogue.SetNext((string)txtMember.Next.Accept(this)); }

            return 0;
        }

        public object Visit_ChoiceMemberExpression(ChoiceMemberExpression choiceMember)
        {
            Dialogue dialogue = GlobalMemory["Dialogue"] as Dialogue;

            dialogue.SetOption((string)choiceMember.Text.Accept(this), (string)choiceMember.Next.Accept(this));

            return 0;
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
    }
}
