using EventScript.Interfaces;

namespace EventScript
{
    public partial class Interpreter
    {
        public object Visit_DialogueExpression(DialogueExpression dialogueExpr)
        {
            dialogueExpr.TextExpression.Accept(this);

            foreach (IExpression member in dialogueExpr.MemberList)
                member.Accept(this);

            return 0;
        }

        public object Visit_DialogueTextExpression(DialogueTextExpression txtMember)
        {
            DialogueData dialogue = Current["BlockData"] as DialogueData;

            dialogue.SetText((string)txtMember.Text.Accept(this));

            if (txtMember.Next != null) { dialogue.SetDialoguePointer((string)txtMember.Next.Accept(this)); }

            return 0;
        }

        public object Visit_DialogueChoiceExpression(DialogueChoiceExpression choiceMember)
        {
            if ((bool)choiceMember.Condition.Accept(this))
            {
                DialogueData dialogue = Current["BlockData"] as DialogueData;
                DialogueChoice choice = new DialogueChoice();

                string text = (string)choiceMember.Text.Accept(this);
                string next = (string)choiceMember.Next.Accept(this);

                choice.SetOption(text, next);

                dialogue.AddChoice(choice);
            }

            return 0;
        }

        public object Visit_DialogueActorExpression(DialogueActorExpression actorExpr)
        {
            DialogueData dialogue = Current["BlockData"] as DialogueData;
            dialogue.SetActorName((string)actorExpr.Text.Accept(this));

            return 0;
        }

        public object Visit_DialogueMoodExpression(DialogueMoodExpression moodExpr)
        {
            DialogueData dialogue = Current["BlockData"] as DialogueData;
            dialogue.SetMood((string)moodExpr.Text.Accept(this));

            return 0;
        }

        public object Visit_DialogueTerminatorExpression(DialogueTerminatorExpression terminator)
        {
            //return null in order for the dialoguePointer to be set as an end pointer
            return null;
        }

        public object Visit_DialogueEndBlockExpression(EndBlockExpression endBlockExpr)
        {
            object value = Current["BlockData"];

            Current = last;
            last = null;

            return value;
        }
    }
}
