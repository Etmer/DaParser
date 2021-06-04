using EventScript.Interfaces;
using System.Collections.Generic;

namespace EventScript.Tests
{
    public class DialogueTestListener
    {
        public bool IsActive { get; private set; } = false;
        public string Text { get; private set; } = null;
        public List<string> ChoiceTexts { get; private set; } = new List<string>();

        public void Register(IDialogueHandler handler) 
        {
            handler.DialogueStartEventHandler += OnDialogueStarted;
            handler.DialogueUpdateEventHandler += OnDialogueUpdated;
            handler.DialogueEndEventHandler += OnDialogueEnded;
        }

        public void Deregister(IDialogueHandler handler)
        {
            handler.DialogueStartEventHandler -= OnDialogueStarted;
            handler.DialogueUpdateEventHandler -= OnDialogueUpdated;
            handler.DialogueEndEventHandler -= OnDialogueEnded;
        }

        private void OnDialogueStarted(DialogueData data) 
        {
            IsActive = true;
            OnDialogueUpdated(data);
        }

        private void OnDialogueUpdated(DialogueData data)
        {
            ChoiceTexts.Clear();
            Text = data.Text;

            foreach (DialogueChoice choice in data.Choices)
                ChoiceTexts.Add(choice.Text);
        }

        private void OnDialogueEnded()
        {
            IsActive = false;
            ChoiceTexts.Clear();
            Text = null;
        }
    }
}
