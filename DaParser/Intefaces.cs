using EventScript.Literals;

namespace EventScript.Interfaces
{
    public interface IDialogueHandler 
    {
        event System.Action<DialogueData> DialogueStartEventHandler;
        event System.Action<DialogueData> DialogueUpdateEventHandler;
        event System.Action DialogueEndEventHandler;
    }
    public interface IDialogueMember : IExpression
    {
        IExpression Text { get; }
        void SetText(IExpression text);
    }
    public interface IExpression { object Accept(IVisitor visitor); }
    public partial interface IVisitor
    {
        object Visit_DialogueExpression(DialogueExpression dialogueExpr);
        object Visit_DialogueTextExpression(DialogueTextExpression txtMember);
        object Visit_DialogueChoiceExpression(DialogueChoiceExpression choiceMember);
        object Visit_DialogueActorExpression(DialogueActorExpression actorExpr);
        object Visit_DialogueMoodExpression(DialogueMoodExpression moodExpr);
        object Visit_DialogueTerminatorExpression(DialogueTerminatorExpression terminator);
        object Visit_DialogueEndBlockExpression(EndBlockExpression endBlockStmt);
    }
    public partial interface IVisitor
    { 
        //Base
        void Visit();
        object Visit_Program(Code code);
        object Visit_ConditionalExpression(ConditionalExpression expr);
        object Visit_BinaryExpression(BinaryExpression expr);
        object Visit_UnaryExpression(UnaryExpression expr);
        object Visit_BlockStatement(BlockStatement block);
        object Visit_DeclarationStatement(DeclarationStatement declStmt);
        object Visit_BlockDeclarationStatement(BlockDeclarationExpression declExpr);
        object Visit_AssignStatement(AssignStatement assignStmt);
        object Visit_Variable(Variable variable);
        object Visit_FunctionCallExpression(FunctionCallExpression func);
        object Visit_ConditionBlock(ConditionBlock condBlock);
        string Visit_StringLiteral(StringLiteral lit);
        double Visit_NumberLiteral(NumberLiteral lit);
        bool Visit_BooleanLiteral(BooleanLiteral lit);
        object Visit_EndExpression(EndExpression endExpr);

    }
}
