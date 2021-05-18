using System;
using System.Collections.Generic;
using System.Text;

namespace EventScript
{
    public class DialogueData
    {
        public string Text { get; private set; } = null;
        public DialoguePointer Next { get; private set; } = null;
        public List<DialogueChoice> Choices = new List<DialogueChoice>();

        public void Reset() { }
    }
}
