using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class DialogueInterpreter : Interpreter
    {
        public System.Action<ErrorCode> OnError;
        public Dialogue currentDialogue { get; private set; } = new Dialogue(5);

        public DialogueInterpreter() : base() 
        {
            GlobalMemory["SetText"] = (System.Action<string>)SetText;
            GlobalMemory["SetChoice"] = (System.Action<string,string>)SetChoice;
            GlobalMemory["GoTo"] = (System.Action<string>)EnterBlockNode;
            GlobalMemory["GetMeMyBool"] = (System.Func<bool>)GetMeMyBool;
        }


        private bool GetMeMyBool()
        {
            return true;
        }
        public void StartDialogue() 
        {
            Interpret();
            EnterBlockNode("Start");
            Print();
        }

        public void UpdateDialogue(int choiceIdx)
        {
            if (choiceIdx < currentDialogue.choices.Count)
            {
                string block = currentDialogue.choices[choiceIdx].Next;
                if (!string.IsNullOrEmpty(block))
                {
                    currentDialogue.Reset();
                    EnterBlockNode(block);
                    Print();
                    return;
                }
            }
            
            OnError?.Invoke(ErrorCode.NoChoiceForInput);
        }

        private void SetText(string text) 
        {
            currentDialogue.SetText(text);
        }

        private void SetChoice(string text, string next)
        {
            currentDialogue.SetOption(text, next);
        }

        private void Print() 
        {
            Console.WriteLine(currentDialogue.Text);

            foreach (DialogueOption option in currentDialogue.choices) 
            {
                if(option.HasInfo)
                    Console.WriteLine($"{"++++"} {option.Text} {"++++"}");
            }
        }

    }
}
