using EventScript.Interfaces;
using System.Collections.Generic;

namespace EventScript.Tests
{
    public class DialogueTestListener : IScriptListener<DialogueData>
    {
        public bool IsActive { get; private set; } = false;
        public string Text { get; private set; } = null;
        public List<string> ChoiceTexts { get; private set; } = new List<string>();

        public void Register(IScriptHandler<DialogueData> handler) 
        {
            handler.DialogueStartEventHandler += OnScriptStarted;
            handler.DialogueUpdateEventHandler += OnScriptUpdated;
            handler.DialogueEndEventHandler += OnScriptEnded;
        }

        public void Deregister(IScriptHandler<DialogueData> handler)
        {
            handler.DialogueStartEventHandler -= OnScriptStarted;
            handler.DialogueUpdateEventHandler -= OnScriptUpdated;
            handler.DialogueEndEventHandler -= OnScriptEnded;
        }

        private void OnScriptStarted(DialogueData data) 
        {
            IsActive = true;
            OnScriptUpdated(data);
        }

        private void OnScriptUpdated(DialogueData data)
        {
            ChoiceTexts.Clear();
            Text = data.Text;

            foreach (DialogueChoice choice in data.Choices)
                ChoiceTexts.Add(choice.Text);
        }

        private void OnScriptEnded()
        {
            IsActive = false;
            ChoiceTexts.Clear();
            Text = null;
        }
    }
}
