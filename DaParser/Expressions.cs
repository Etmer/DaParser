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
        public BlockStatement BlockStatement {get; private set;}

        public override object Accept(IVisitor visitor) { return visitor.Visit_Program(this); }
        public void SetBlockStatement(BlockStatement stmt) { BlockStatement = stmt; }
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
        public Literal Name { get; private set; }
        public BlockStatement BlockStatement;

        public BlockVariable(Literal name) { Name = name; }

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

    public class FALSE_Expression : IExpression
    {
        public object Accept(IVisitor visitor) { return false; }
    }

    public class DialogueExpression : IExpression
    {
        public TextMemberExpression TextExpression { get; private set; }
        public List<ChoiceMemberExpression> ChoiceExpressionList { get; private set; } = new List<ChoiceMemberExpression>();

        public void SetTextExpression(TextMemberExpression expr) { TextExpression = expr; }
        public void AddChoiceExpression(ChoiceMemberExpression expr) { ChoiceExpressionList.Add(expr); }
        public void AddChoiceExpression(List<ChoiceMemberExpression> expr) { if(expr != null) ChoiceExpressionList.AddRange(expr); }

        public object Accept(IVisitor visitor) { return visitor.Visit_DialogueExpression(this); }
    }

    public abstract class DialogueMember : IExpression
    {
        public IExpression Text { get; protected set; } = null;
        public IExpression Next { get; protected set; } = null;
        public abstract object Accept(IVisitor visitor);

        public void SetText(IExpression text) { Text = text; }
        public void SetNext(IExpression next) { Next = next; }
    }

    public class ChoiceMemberExpression : DialogueMember
    {
        public IExpression Condition { get; private set; }
        public override object Accept(IVisitor visitor) { return visitor.Visit_ChoiceMemberExpression(this); }
        public void SetCondition(IExpression condition) { Condition = condition; }
    }

    public class TextMemberExpression : DialogueMember
    {
        public override object Accept(IVisitor visitor) { return visitor.Visit_TextMemberExpression(this); }
    }
}
