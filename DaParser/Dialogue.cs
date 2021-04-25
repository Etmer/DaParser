using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Dialogue
    {
        public List<DialogueOption> choices;

        private string defaultExit = null;
        private int index = 0;

        public Dialogue(int choiceAmount) 
        {
            choices = new List<DialogueOption>();
            for (int i = 0; i < choiceAmount; i++)
                choices.Add(new DialogueOption());
        }

        public Dialogue()
        {
            choices = new List<DialogueOption>();
        }


        /// <summary>
        /// The displayed text of the dialogue
        /// </summary>
        public string Text { get; private set; }

        public void SetText(string text) { Text = text; }

        /// <summary>
        /// Sets the next available Option text and next string, increases the index of the currently inspected dialogue option.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="next"></param>
        public void SetOption(string text, string next, bool createIfNotExist = true)
        {
            if (index > choices.Count)
                if (createIfNotExist)
                {
                    DialogueOption newOption = new DialogueOption();
                    newOption.Set(text, next);
                }

            DialogueOption option = choices[index++];
            option.Set(text, next);
        }

        /// <summary>
        /// Sets the name of the node that follows ifno choicesare available
        /// </summary>
        /// <param name="value"></param>
        public void SetDefaultExit(string value) 
        {
            defaultExit = value;
        }

        /// <summary>
        /// Resets dialogue text and all options
        /// </summary>
        public void Reset() 
        {
            index = 0;
            Text = "";
            defaultExit = null;

            foreach (DialogueOption choice in choices)
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
}
