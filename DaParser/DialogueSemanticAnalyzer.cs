using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript
{
    public partial class SemanticAnalyzer
    {
        public object Visit_DialogueChoiceExpression(DialogueChoiceExpression choiceMember)
    {
            string name = choiceMember.Next.Accept(this) as string;

            if(name != null)
                if (!currentTable.LookUp(name, out ISymbol symbol))
                    throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, ((NodeBase)choiceMember.Next).Token);
           
            return 0;
        }

        public object Visit_DialogueExpression(DialogueExpression dialogueExpr)
        {
            dialogueExpr.TextExpression.Accept(this);

            foreach (IExpression expr in dialogueExpr.MemberList)
                expr.Accept(this);

            return 0;
        }

        public object Visit_DialogueTextExpression(DialogueTextExpression txtMember)
        {
            if (txtMember.Next != null)
            {
                string name = txtMember.Next.Accept(this) as string;

                if(name != null)
                    if (!currentTable.LookUp(name, out ISymbol symbol))
                        throw RaiseError(ScriptErrorCode.ID_NOT_FOUND, ((NodeBase)txtMember.Next).Token);
            }
            return 0;
        }

        public object Visit_DialogueActorExpression(DialogueActorExpression actorExpr)
        {
            return 0;
        }

        public object Visit_DialogueMoodExpression(DialogueMoodExpression moodExpr)
        {
            return 0;
        }

        public object Visit_DialogueTerminatorExpression(DialogueTerminatorExpression terminator)
        {
            return 0;
        }

        public object Visit_DialogueEndBlockExpression(EndBlockExpression endBlockStmt)
        {
            return 0;
        }
    }
}