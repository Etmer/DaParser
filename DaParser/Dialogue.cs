using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Dialogue
    {
        public List<DialogueOption> choices;
        private int index = 0;

        public Dialogue(int choiceAmount) 
        {
            choices = new List<DialogueOption>();
            for (int i = 0; i < choiceAmount; i++)
                choices.Add(new DialogueOption());
        }

        public string Text { get; private set; }

        public void SetText(string text) { Text = text; }

        public void SetOption(string text, string next)
        {
            DialogueOption option = choices[index++];
            option.Set(text, next);
        }

        public void Reset() 
        {
            index = 0;
            Text = "";

            foreach (DialogueOption choice in choices)
                choice.Reset();
        }
    }
    public class DialogueOption
    {
        public bool HasInfo { get { return !string.IsNullOrEmpty(Text); } }
        public string Text { get; private set; }
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
