using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Dialogue
    {
        /// <summary>
        /// The displayed text of the dialogue
        /// </summary>
        public string Text { get; private set; }
        public List<DialogueOption> Choices;

        private DialoguePointer dialoguePointer = new DialoguePointer();
        private int index = 0;

        public Dialogue(int choiceAmount) 
        {
            Choices = new List<DialogueOption>();
            for (int i = 0; i < choiceAmount; i++)
                Choices.Add(new DialogueOption());
        }

        public Dialogue()
        {
            Choices = new List<DialogueOption>();
        }

        public DialogueOption GetChoice(int index) 
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
                    DialogueOption newOption = new DialogueOption();
                    newOption.Set(text, next);
                }

            DialogueOption option = Choices[index++];
            option.Set(text, next);
        }

        /// <summary>
        /// Sets the name of the node that follows if no choices are available
        /// </summary>
        /// <param name="value"></param>
        public void SetNext(string value, DialogueExitMode exitMode) 
        {
            dialoguePointer.Set(value, exitMode);
        }

        /// <summary>
        /// Resets dialogue text and all options
        /// </summary>
        public void Reset() 
        {
            index = 0;
            Text = "";
            dialoguePointer.Reset();

            foreach (DialogueOption choice in Choices)
                choice.Reset();
        }
    }
    public class DialogueOption
    {
        public bool HasInfo { get { return !string.IsNullOrEmpty(Text); } }

        /// <summary>
        /// The Displayed Text in the choice box
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The name of the node to visist after this choice was picked
        /// </summary>
        public string Next { get; private set; }

        public void Set(string text, string next)
        {
            Text = text;
            Next = next;
        }

        public void Reset() 
        { 
            Text = "";
            Next = "";
        }
    }

    public class DialoguePointer 
    {
        public DialogueExitMode ExitMode { get; private set; }
        public string Next { get; private set; } = null;

        public void Set(string next, DialogueExitMode exitMode) 
        {
            Next = next;
            ExitMode = exitMode;
        }

        public void Reset() 
        {
            ExitMode = DialogueExitMode.INVALID;
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
