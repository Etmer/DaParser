using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class DialogueTester 
    {
        public event System.Action<string, List<DialogueChoice>> OnStart;
        public event System.Action<string, List<DialogueChoice>> OnUpdate;
        public event System.Action OnEnd;

        public System.Action<ErrorCode> OnError;

        private Interpreter interpreter = new Interpreter();

        public void Start() 
        {
            interpreter.GlobalMemory["CallMyCall"] = (System.Action)End;

            interpreter.Interpret();
            Dialogue dialogue = interpreter.GlobalMemory["Dialogue"] as Dialogue;

            interpreter.EnterBlockNode("Start");
            OnStart?.Invoke(dialogue.Text,dialogue.Choices);

            while (true) 
            {
                string input = Console.ReadLine();


                if (!dialogue.DefaultExit.HasInfo)
                {
                    if (int.TryParse(input, out int index))
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
            Dialogue dialogue = interpreter.GlobalMemory["Dialogue"] as Dialogue;

            DialoguePointer choice = dialogue.GetChoice(index);
            string next = choice.Next;

            dialogue.Reset();
            interpreter.EnterBlockNode(next);
            Update();
        }

        private void SelectDefaultExit()
        {
            Dialogue dialogue = interpreter.GlobalMemory["Dialogue"] as Dialogue;

            string next = dialogue.DefaultExit.Next;

            dialogue.Reset();
            interpreter.EnterBlockNode(next);
            Update();
        }

        private void Update()
        {
            Dialogue dialogue = interpreter.GlobalMemory ["Dialogue"] as Dialogue;

            OnUpdate?.Invoke(dialogue.Text, dialogue.Choices);
        }

        private void End() 
        {
            int i = 0;
            OnEnd?.Invoke();
        }

    }
}
