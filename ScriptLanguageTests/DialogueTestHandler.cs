using System;
using EventScript.Interfaces;

namespace EventScript.Tests
{
    class DialogueTestHandler : IScriptHandler<DialogueData>
    {
        public DialogueData CurrentData { get; private set; }

        private DialogueScript dialogueScript = new DialogueScript();

        public event Action<DialogueData> DialogueStartEventHandler;
        public event Action<DialogueData> DialogueUpdateEventHandler;
        public event Action DialogueEndEventHandler;

        public DialogueTestHandler() { ; }
        ~DialogueTestHandler() {  }

        public void ChooseOption(int index) 
        {
            string next = null;

            if (CurrentData.Choices.Count == 0)
                next = CurrentData.DefaultExit.Next;
            else
                next = CurrentData.Choices[index].Next;

            if (next == null)
                End();
            else
                UpdateData((DialogueData)dialogueScript.Interpreter.EnterBlockNode(next));
        }

        public void Start()
        {
            CurrentData = (DialogueData)dialogueScript.Interpreter.EnterBlockNode("Start");
            DialogueStartEventHandler?.Invoke(CurrentData);
        }

        public void UpdateData(DialogueData data)
        {
            CurrentData = data;
            DialogueUpdateEventHandler?.Invoke(CurrentData);
        }

        public void End()
        {
            DialogueEndEventHandler?.Invoke();
        }

        public void ReadScript(string script) 
        {
            dialogueScript.Parse(script);
            dialogueScript.Interpreter.PreVisit();
        }
    }
}
