using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class DialogueInterpreter : Interpreter
    {
        public event System.Action<string, List<DialogueOption>> OnStart;
        public event System.Action<string, List<DialogueOption>> OnUpdate;
        public event System.Action OnEnd;

        public System.Action<ErrorCode> OnError;
        private Dialogue dialogue { get; set; } = new Dialogue(5);

        public DialogueInterpreter() : base()
        {
            GlobalMemory["Dialogue"] = dialogue;
        }

        public void Start() 
        {
            Interpret();
            EnterBlockNode("Start");
            OnStart?.Invoke(dialogue.Text,dialogue.Choices);

            while (true) 
            {
                int index = int.Parse(Console.ReadLine());
                SelectChoice(index);
            }
        }

        public void SelectChoice(int index) 
        {
            DialogueOption choice = dialogue.GetChoice(index);
            string next = choice.Next;

            dialogue.Reset();
            EnterBlockNode(next);
            Update();
        }

        private void Update()
        {
            OnUpdate?.Invoke(dialogue.Text, dialogue.Choices);
        }

        private void End() 
        {
            OnEnd?.Invoke();
        }

        private void Print(string text, List<DialogueOption> choices) 
        {
            Console.WriteLine(text);

            foreach (DialogueOption choice in choices)
                Console.WriteLine($"+++++{choice.Text}+++++");
        }

    }
}
