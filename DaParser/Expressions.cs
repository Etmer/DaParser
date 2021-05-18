using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript
{
    public abstract class Statement : IExpression
    {
        public abstract object Accept(IVisitor visitor);
    }

    public class DeclarationStatement : Statement
    {
        public Variable DeclaredVariable { get; private set; }
        public IExpression Expression { get; private set; }
        public override object Accept(IVisitor visitor) { return visitor.Visit_DeclarationStatement(this); }

        public void SetVariable(Variable variable) { DeclaredVariable = variable; }
        public void SetExpression(IExpression expression) { Expression = expression; }
    }

    public class Code : Statement
    {
        public BlockStatement BlockState {get; private set;}

        public override object Accept(IVisitor visitor) { return visitor.Visit_Program(this); }
    }

    public class BlockStatement : Statement
    {
        public List<IExpression> Satements { get; } = new List<IExpression>();

        public override object Accept(IVisitor visitor) { return visitor.Visit_BlockStatement(this); }
        public void Append(IExpression statement) { Satements.Add(statement); }
        public void AppendRange(List<IExpression> statementList) { Satements.AddRange(statementList); }
    }

    public class BlockVariable : Statement
    {
        public string Name { get; private set; }
        public BlockStatement BlockStatement;

        public BlockVariable(string name) { Name = name; }

        public override object Accept(IVisitor visitor) { return visitor.Visit_BlockVariable(this); }
        public void SetBlockStatement(BlockStatement blockStmt) { BlockStatement = blockStmt; }
    }

    public class ConditionalExpression : Statement
    {
        public List<ConditionBlock> conditionBlocks = new List<ConditionBlock>();
        public override object Accept(IVisitor visitor) { return visitor.Visit_ConditionalExpression(this); }

        public void AddConditionBlock(ConditionBlock block) { conditionBlocks.Add(block); }
        public void AddConditionBlocks(List<ConditionBlock> blocks) { conditionBlocks.AddRange(blocks); }
    }

    public class ConditionBlock : IExpression
    {
        public IExpression Condition { get; private set; }
        public BlockStatement BlockStatement { get; private set; }
        
        public object Accept(IVisitor visitor) { return visitor.Visit_ConditionBlock(this); }
        public void SetCondition(IExpression condition) { Condition = condition; }
        public void SetBlockStatement(BlockStatement blockStatement) { BlockStatement = blockStatement; }
    }

    public class AssignStatement : IExpression
    {
        public Variable Variable { get; private set; }
        public IExpression Expression { get; private set; }
        public object Accept(IVisitor visitor) { return visitor.Visit_AssignStatement(this); }

        public void SetVariable(Variable variable) { Variable = variable; }
        public void SetExpression(IExpression expression) { Expression = expression; }
    }

    public abstract class OperatorExpression
    {
        public TokenType OperatorType { get; private set; }
        public void SetOperatorType(TokenType operatorType) { OperatorType = operatorType; }

    }

    public class BinaryExpression : OperatorExpression, IExpression
    {
        public IExpression Left { get; private set; }
        public IExpression Right { get; private set; }

        public object Accept(IVisitor visitor) { return visitor.Visit_BinaryExpression(this); }

        public void SetLeft(IExpression left) { Left = left; }
        public void SetRight(IExpression right) { Right = right; }
    }

    public class UnaryExpression : OperatorExpression, IExpression
    {
        public IExpression Expression { get; private set; }
        public object Accept(IVisitor visitor) { return visitor.Visit_UnaryExpression(this); }
        public void SetExpression(IExpression expression) { Expression = expression; }

    }

    public class FunctionCallExpression : IExpression
    {
        public string Callee;
        public List<IExpression> Arguments = new List<IExpression>();
        public FunctionCallExpression(string callee)
        {
            Callee = callee;
        }

        public void AddArgument(IExpression argument)
        {
            Arguments.Add(argument);
        }
        public object Accept(IVisitor visitor)
        {
            return visitor.Visit_FunctionExpression(this);
        }
    }

    public class TRUE_Expression : IExpression
    {
        public object Accept(IVisitor visitor) { return true; }
    }
    public class False_Expression : IExpression
    {
        public object Accept(IVisitor visitor) { return false; }
    }
}
