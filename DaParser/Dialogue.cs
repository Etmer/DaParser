using System.Collections.Generic;

namespace EventScript
{
    public class Dialogue : IValue
    {
        public event System.Action DialogueEndEventHandler;

        /// <summary>
        /// The displayed text of the dialogue
        /// </summary>
        public string Text { get; private set; }

        public DialoguePointer DefaultExit = new DialoguePointer();
        public List<DialogueChoice> Choices;
        public bool HasEnded { get; private set; }

        private int index = 0;

        public Dialogue(int choiceAmount) 
        {
            Choices = new List<DialogueChoice>();
            for (int i = 0; i < choiceAmount; i++)
                Choices.Add(new DialogueChoice());
        }

        public Dialogue()
        {
            Choices = new List<DialogueChoice>();
        }

        public DialoguePointer GetChoice(int index) 
        {
            return Choices[index];
        }

        public void SetText(string text) { Text = text; }

        /// <summary>
        /// Sets the next available Option text and next string, increases the index of the currently inspected dialogue option.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="next"></param>
        public void SetOption(string text, string next, bool createIfNotExist = true)
        {
            if (index > Choices.Count)
                if (createIfNotExist)
                {
                    DialogueChoice newOption = new DialogueChoice();
                    newOption.SetOption(text, next);
                }

            DialogueChoice option = Choices[index++];
            option.SetOption(text, next);
        }

        /// <summary>
        /// Sets the name of the node that follows if no choices are available
        /// </summary>
        /// <param name="value"></param>
        public void SetNext(string value) 
        {
            DefaultExit.SetNext(value);
        }

        /// <summary>
        /// Resets dialogue text and all options
        /// </summary>
        public void Reset() 
        {
            index = 0;
            Text = null;
            DefaultExit.Reset();

            foreach (DialogueChoice choice in Choices)
                choice.Reset();
        }

        public void End() 
        {
            DialogueEndEventHandler?.Invoke();
        }

        public object GetValue(List<object> arguments = null)
        {
            return this;
        }
    }
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
        public virtual bool HasInfo { get { return Next != null; } }
        /// <summary>
        /// The name of the node to visist after this choice was picked
        /// </summary>
        public string Next { get; protected set; } = null;

        public void SetNext(string next) 
        {
            Next = next;
        }

        public virtual void Reset() 
        {
            Next = null;
        }
    }
    
    public enum DialogueExitMode 
    {
        INVALID,
        End,
        Running
    }

}
