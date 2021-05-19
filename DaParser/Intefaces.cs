using EventScript.Literals;

namespace EventScript.Interfaces
{

    public interface IExpression : IVisitable { }

    public interface IParsable
    {
        void Parse(NodeParser parser);
    }

    public interface IVisitor
    {
        //Base
        object Visit_Program(Code code);
        object Visit_ConditionalExpression(ConditionalExpression expr);
        object Visit_BinaryExpression(BinaryExpression expr);
        object Visit_UnaryExpression(UnaryExpression expr);
        object Visit_BlockStatement(BlockStatement block);
        object Visit_BlockVariable(BlockVariable blockVar);
        object Visit_DeclarationStatement(DeclarationStatement declStmt);
        object Visit_AssignStatement(AssignStatement assignStmt);
        object Visit_Variable(Variable variable);
        object Visit_FunctionExpression(FunctionCallExpression func);
        object Visit_ConditionBlock(ConditionBlock condBlock);

        //Dialogue
        object Visit_DialogueExpression(DialogueExpression dialogueExpr);
        object Visit_TextMemberExpression(TextMemberExpression txtMember);
        object Visit_ChoiceMemberExpression(ChoiceMemberExpression choiceMember);
    }
    public interface IVisitable
    {
        object Accept(IVisitor visitor);
    }
}
