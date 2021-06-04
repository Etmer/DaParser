using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript
{
    public class NodeBase 
    {
        public Token Token { get; private set; }
        public void SetToken(Token token) { Token = token; }
    }

    public abstract class Statement : NodeBase, IExpression
    {
        public abstract object Accept(IVisitor visitor);
    }

    public class DeclarationStatement : Statement
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public IExpression Expression { get; private set; }
        public override object Accept(IVisitor visitor) { return visitor.Visit_DeclarationStatement(this); }

        public void SetName(string name) { Name = name; }
        public void SetType(string type) { Type = type; }
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

    public class ConditionalExpression : Statement
    {
        public ConditionBlock IfBlock { get; set; }
        public ConditionalExpression ElseBlock { get; set; }
        public override object Accept(IVisitor visitor) { return visitor.Visit_ConditionalExpression(this); }

        public void SetIfBlock(ConditionBlock block) { IfBlock = block; }
        public void SetElseBlock(ConditionalExpression expr) { ElseBlock = expr; }
    }

    public class ConditionBlock : NodeBase, IExpression
    {
        public int Precedence { get; private set; } = 0;
        public IExpression Condition { get; private set; }
        public BlockStatement BlockStatement { get; private set; }
        
        public object Accept(IVisitor visitor) { return visitor.Visit_ConditionBlock(this); }
        public void SetCondition(IExpression condition) { Condition = condition; }
        public void SetBlockStatement(BlockStatement blockStatement) { BlockStatement = blockStatement; }
        public void SetPrecedence(int precedence) { Precedence = precedence; }
    }

    public class AssignStatement : NodeBase, IExpression
    {
        public Variable Variable { get; private set; }
        public IExpression Expression { get; private set; }
        public object Accept(IVisitor visitor) { return visitor.Visit_AssignStatement(this); }

        public void SetVariable(Variable variable) { Variable = variable; }
        public void SetExpression(IExpression expression) { Expression = expression; }
    }

    public abstract class OperatorExpression : NodeBase
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

    public class FunctionCallExpression : NodeBase, IExpression
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
            return visitor.Visit_FunctionCallExpression(this);
        }
    }

    public class TRUE_Expression : NodeBase, IExpression
    {
        public object Accept(IVisitor visitor) { return true; }
    }

    public class FALSE_Expression : IExpression
    {
        public object Accept(IVisitor visitor) { return false; }
    }

    public class DialogueExpression : NodeBase, IExpression
    {
        public DialogueTextExpression TextExpression { get; private set; }
        public List<IDialogueMember> MemberList { get; private set; } = new List<IDialogueMember>();

        public void SetTextExpression(DialogueTextExpression expr) { TextExpression = expr; }
        public void AddChoiceExpression(IDialogueMember expr) { MemberList.Add(expr); }
        public void AddChoiceExpression(List<IDialogueMember> expr) { if(expr != null) MemberList.AddRange(expr); }

        public object Accept(IVisitor visitor) { return visitor.Visit_DialogueExpression(this); }
    }

    public abstract class DialogueMemberBase : NodeBase, IDialogueMember
    {
        public IExpression Text { get; protected set; }
        public abstract object Accept(IVisitor visitor);
        public void SetText(IExpression text)  { Text = text; }
    }

    public class DialogueChoiceExpression : DialogueTextExpression
    {
        public IExpression Condition { get; private set; }
        public override object Accept(IVisitor visitor) { return visitor.Visit_DialogueChoiceExpression(this); }
        public void SetCondition(IExpression condition) { Condition = condition; }
    }

    public class DialogueTextExpression : DialogueMemberBase
    {
        public IExpression Next { get; protected set; } = null;
        public void SetNext(IExpression next) { Next = next; }
        public override object Accept(IVisitor visitor) {  return visitor.Visit_DialogueTextExpression(this); }
    }

    public class DialogueActorExpression : DialogueMemberBase
    {
        public override object Accept(IVisitor visitor) { return visitor.Visit_DialogueActorExpression(this); }
    }

    public class DialogueMoodExpression : DialogueMemberBase
    {
        public override object Accept(IVisitor visitor) { return visitor.Visit_DialogueMoodExpression(this); }
    }

    public class DialogueTerminatorExpression : NodeBase, IExpression
    {
        public object Accept(IVisitor visitor) { return visitor.Visit_DialogueTerminatorExpression(this); }
    }

    public class EndExpression : NodeBase, IExpression
    {
        public object Accept(IVisitor visitor) { return visitor.Visit_EndExpression(this); }
    }

    public class BlockDeclarationExpression : DeclarationStatement 
    {
        public override object Accept(IVisitor visitor) { return visitor.Visit_BlockDeclarationStatement(this); }
    }

    public class EndBlockExpression : NodeBase, IExpression
    {
        public object Accept(IVisitor visitor) { return visitor.Visit_DialogueEndBlockExpression(this); }
    }
}
