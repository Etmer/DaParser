using System;
using System.Collections.Generic;
using System.Text;

namespace EventScript
{
    public class DialogueTester 
    {
        private static Script script = new Script();

        public event System.Action<string, List<DialogueChoice>> OnStart;
        public event System.Action<string, List<DialogueChoice>> OnUpdate;
        public event System.Action OnEnd;

        public System.Action<ErrorCode> OnError;

        public void Start(string input) 
        {
            Dialogue dialogue = new Dialogue(5);
            script.Interpreter.GlobalMemory["Dialogue"] = dialogue;
            script.Interpreter.GlobalMemory["CallMyCall"] = (System.Action<string, string>)End;

            script.Parse(input);
            script.Interpreter.Visit();
           
            script.Interpreter.EnterBlockNode("Start");
            OnStart?.Invoke(dialogue.Text,dialogue.Choices);

            while (true) 
            {
                string consoleInput = Console.ReadLine();


                if (!dialogue.DefaultExit.HasInfo)
                {
                    if (int.TryParse(consoleInput, out int index))
                        SelectChoice(index);
                }
                else
                {
                    SelectDefaultExit();
                }
            }
        }

        public void SelectChoice(int index)
        {
            Dialogue dialogue = script.Interpreter.GlobalMemory["Dialogue"] as Dialogue;

            DialoguePointer choice = dialogue.GetChoice(index);
            string next = choice.Next;

            dialogue.Reset();
            script.Interpreter.EnterBlockNode(next);
            Update();
        }

        private void SelectDefaultExit()
        {
            Dialogue dialogue = script.Interpreter.GlobalMemory["Dialogue"] as Dialogue;

            string next = dialogue.DefaultExit.Next;

            dialogue.Reset();
            script.Interpreter.EnterBlockNode(next);
            Update();
        }

        private void Update()
        {
            Dialogue dialogue = script.Interpreter.GlobalMemory ["Dialogue"] as Dialogue;

            OnUpdate?.Invoke(dialogue.Text, dialogue.Choices);
        }

        private void End(string s, string s1) 
        {
            OnEnd?.Invoke();
        }

    }
}
