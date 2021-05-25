using NUnit.Framework;
using EventScript;

namespace ScriptLanguageTests
{
    public class SimpleTest
    {
        private string input = @"
               
            [Start]
                {Text = 'Test one'}
                    {Choice = 'Test choice one' => 'End'}
                    {Choice = 'Test choice two' => 'End'}
                    {Choice = 'Test choice three' => 'End'};
            end

            [End]
                 {Text = 'Test one' => 'Start'}; 
            end
        ";

        [SetUp]
        public void Setup() { }

        [Test]
        public void Test()
        {
            Script script = new Script();

            script.Parse(input);
            script.Interpreter.Visit();

            script.Interpreter.EnterBlockNode("Start");
            Dialogue dialogue = (Dialogue)script.Interpreter.GlobalMemory["Dialogue"];

            Assert.IsTrue(dialogue.Text == "Test one");

            Assert.IsTrue(dialogue.Choices[0].Text == "Test choice one");
            Assert.IsTrue(dialogue.Choices[1].Text == "Test choice two");
            Assert.IsTrue(dialogue.Choices[2].Text == "Test choice three");

            Assert.IsTrue(dialogue.Choices[0].Next == "End");
            Assert.IsTrue(dialogue.Choices[1].Next == "End");
            Assert.IsTrue(dialogue.Choices[2].Next == "End");

            Assert.Pass();
        }
    }
}