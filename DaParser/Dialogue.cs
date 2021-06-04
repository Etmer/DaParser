using System.Collections.Generic;

namespace EventScript
{
    public class DialogueChoice : DialoguePointer
    {
        public override bool HasInfo { get { return Text != null && Next != null; } }

        /// <summary>
        /// The Displayed Text in the choice box
        /// </summary>
        public string Text { get; private set; }

        public void SetOption(string text, string next)
        {
            Text = text;
            SetNext(next);
        }

        public override void Reset()
        {
            base.Reset();
            Text = null;
        }
    }

    public class DialoguePointer
    {
        public bool Exit { get; private set; } = false;
        public virtual bool HasInfo { get { return Next != null || Exit == true; } }
        /// <summary>
        /// The name of the node to visist after this choice was picked
        /// </summary>
        public string Next { get; protected set; } = null;

        public void SetNext(string next) 
        {
            if (next == null)
                Exit = true;

            Next = next;
        }

        public virtual void Reset() 
        {
            Next = null;
            Exit = false;
        }
    }

    public class DialogueData : IValue
    {
        public string Text { get; private set; }
        public string ActorName { get; private set; } = null;
        public string Mood { get; private set; } = null;

        public DialoguePointer DefaultExit = new DialoguePointer();
        public List<DialogueChoice> Choices = new List<DialogueChoice>(5);

        public void SetText(string text) { Text = text; }
        public void SetActorName(string name) { ActorName = name; }
        public void SetMood(string mood) { Mood = mood; }
        public void AddChoice(DialogueChoice choice) { Choices.Add(choice); }
        public void SetDialoguePointer(string next) { DefaultExit.SetNext(next); }
        public void Reset() 
        {
            DefaultExit.Reset();
            Choices.Clear();

            DefaultExit = null;
            Mood = null;
            Text = null;
            ActorName = null;
        }

        public object GetValue(List<object> arguments = null)
        {
            return this;
        }
    }
}


