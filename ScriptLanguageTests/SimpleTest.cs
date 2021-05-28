using NUnit.Framework;
using EventScript;

namespace ScriptLanguageTests
{
    public class SimpleTest
    {
        [SetUp]
        public void Setup() { }

        #region Test 1
        private string script_1 = @"
               
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

        [Test]
        public void Test()
        {
            Script script = new Script();

            script.Parse(script_1);
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

        #endregion

        #region Test 2

        private string script_2 = @"
               
            double d = 20 * 5 + 3;
            double d1 = 20 * (5 + 3);
            double d2 = 20 * (8 + 3 * 4);
            double d3 = 20 * ((8 + 2) * 4);
        ";

        [Test]
        public void Test2()
        {
            Script script = new Script();

            script.Parse(script_2);
            script.Interpreter.Visit();

            double d = ((DoubleValue)script.Interpreter.GlobalMemory["d"]).Value;
            double d1 = ((DoubleValue)script.Interpreter.GlobalMemory["d1"]).Value;
            double d2 = ((DoubleValue)script.Interpreter.GlobalMemory["d2"]).Value;
            double d3 = ((DoubleValue)script.Interpreter.GlobalMemory["d3"]).Value;

            Assert.IsTrue(d == 103);
            Assert.IsTrue(d1 == 160);
            Assert.IsTrue(d2 == 400);
            Assert.IsTrue(d3 == 800);

            Assert.Pass();
        }

        #endregion

        #region Test 3
        private string script_3 = @"
               
            double d = 20;
            
            [Start]
                {Text = 'Test one'}
                    (d < 20) {Choice = 'Test choice one' => 'End'}
                    {Choice = 'Test choice two' => 'End'}
                    {Choice = 'Test choice three' => 'End'};
            end

            [End]
                 {Text = 'Test one' => 'Start'}; 
            end
        ";

        [Test]
        public void Test3()
        {
            Script script = new Script();

            script.Parse(script_3);
            script.Interpreter.Visit();

            script.Interpreter.EnterBlockNode("Start");
            Dialogue dialogue = (Dialogue)script.Interpreter.GlobalMemory["Dialogue"];

            Assert.IsTrue(dialogue.Text == "Test one");

            Assert.IsTrue(dialogue.Choices[0].Text == "Test choice two");
            Assert.IsTrue(dialogue.Choices[1].Text == "Test choice three");

            Assert.IsTrue(dialogue.Choices[0].Next == "End");
            Assert.IsTrue(dialogue.Choices[1].Next == "End");

            Assert.Pass();
        }

        #endregion

        #region Test 4

        private string script_4 = @"
               
            [Start]
                {Text = 'Test one'}
                    {Choice = 'Test choice one' => 'End'};
            end
        ";

        [Test]
        public void Test4()
        {
            Script script = new Script();

            ScriptException ex = Assert.Throws<ScriptException>(() => { script.Parse(script_4); });
            Assert.That(ex.Message, Is.EqualTo("Semantic Error: Undeclared ID: End at 5.31"));

            Assert.Pass();
        }

        #endregion

        #region Test 5

        private string script_5 = @"
               
            double d = 10;

            [Start]
                if(d == 10) then
                    d = 150;
                elif(d < 10) then
                    d = 4;
                else
                    d = 5;
                elif(d < 40) then
                    d = 6;
                end
            end
        ";

        [Test]
        public void ConditionTest_1()
        {
            Script script = new Script();

            ScriptException ex =Assert.Throws<ScriptException>(() => script.Parse(script_5));
            Assert.That(ex.Message, Is.EqualTo("Parsing Error: Unexpected Token: ELSEIF at 12.18"));

            Assert.Pass();
        }

        #endregion

        #region Test 6

        [Test]
        public void ConditionTest_2()
        {
            double d = 0;

            d = GetValueFromInput("10");
            Assert.True(d == 1);

            d = GetValueFromInput("5");
            Assert.True(d == 2);

            d = GetValueFromInput("20");
            Assert.True(d == 3);

            d = GetValueFromInput("41");
            Assert.True(d == 4);

            d = GetValueFromInput("40");
            Assert.True(d == 5);

            Assert.Pass();

            double GetValueFromInput(string input) 
            {
                string script_6 = $@"
               
                double d = {input};

                [Start]
                    if(d == 10) then
                        d = 1;
                    elif(d < 10) then
                        d = 2;
                    elif(d == 20) then
                        d = 3;
                    elif(d > 40) then
                        d = 4;
                    else
                        d = 5;
                    end
                end
            ";

                Script script = new Script();
                script.Parse(script_6);
                script.Interpreter.Visit();
                script.Interpreter.EnterBlockNode("Start");
                return ((DoubleValue)script.Interpreter.GlobalMemory["d"]).Value; 
            }

        }

        #endregion

    }
}