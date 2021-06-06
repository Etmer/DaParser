using NUnit.Framework;
using EventScript;

namespace EventScript.Tests
{
    public class SimpleTest
    {
        [SetUp]
        public void Setup() { }

        #region Test 1
        private string script_1 = @"
               
            [Start]
                {Text = 'Test one'}
                    {Choice = 'Test choice one' => 'Test'}
                    {Choice = 'Test choice two' => 'Test'}
                    {Choice = 'Test choice three' => 'Test'};

            end

            [Test]
                 {Text = 'Test one' => 'Start'}; 
            end
        ";

        [Test]
        public void Test()
        {
            DialogueScript script = new DialogueScript();

            script.Parse(script_1);
            script.Interpreter.PreVisit();

            DialogueData data = (DialogueData)script.Interpreter.EnterBlockNode("Start");

            Assert.IsTrue(data.Text == "Test one");

            Assert.IsTrue(data.Choices[0].Text == "Test choice one");
            Assert.IsTrue(data.Choices[1].Text == "Test choice two");
            Assert.IsTrue(data.Choices[2].Text == "Test choice three");

            Assert.IsTrue(data.Choices[0].Next == "Test");
            Assert.IsTrue(data.Choices[1].Next == "Test");
            Assert.IsTrue(data.Choices[2].Next == "Test");

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
            BehaviourScript script = new BehaviourScript();

            script.Parse(script_2);
            script.Interpreter.PreVisit();

            double d = ((DoubleValue)script.Interpreter.Current["d"]).Value;
            double d1 = ((DoubleValue)script.Interpreter.Current["d1"]).Value;
            double d2 = ((DoubleValue)script.Interpreter.Current["d2"]).Value;
            double d3 = ((DoubleValue)script.Interpreter.Current["d3"]).Value;

            Assert.IsTrue(d == 103);
            Assert.IsTrue(d1 == 160);
            Assert.IsTrue(d2 == 400);
            Assert.IsTrue(d3 == 800);

            Assert.Pass();
        }

        #endregion

        #region Test 3

        string script_3 = @"
               
                double d = 21;
            
                [Start]
                    {Text = 'Test one'}
                        (d < 20) ?  {Choice = 'Test choice one' => 'End'}
                         {Choice = 'Test choice two' => 'End'}
                         {Choice = 'Test choice three' => 'End'};
                    end

                [End]
                         {Text = 'Test one' => 'Start'}; 
                end
                ";

        string script_3_1 = @"
               
                 double d = 1;
            
                [Start]
                    {Text = 'Test one'}
                        (d < 20) ?  {Choice = 'Test choice one' => 'End'}
                         {Choice = 'Test choice two' => 'End'}
                         {Choice = 'Test choice three' => 'End'};
                    end

                [End]
                         {Text = 'Test one' => 'Start'}; 
                end";


        [Test]
        public void Test3()
        {
            DialogueScript script = new DialogueScript();
            script.Parse(script_3);
            script.Interpreter.PreVisit();

            DialogueData data = (DialogueData)script.Interpreter.EnterBlockNode("Start");

            Assert.IsTrue(data.Text == "Test one");

            Assert.IsTrue(data.Choices[0].Text == "Test choice two");
            Assert.IsTrue(data.Choices[1].Text == "Test choice three");

            Assert.IsTrue(data.Choices[0].Next == "End");
            Assert.IsTrue(data.Choices[1].Next == "End");

            DialogueScript script2 = new DialogueScript();
            script2.Parse(script_3_1);
            script2.Interpreter.PreVisit();

            data = (DialogueData)script2.Interpreter.EnterBlockNode("Start");

            Assert.IsTrue(data.Text == "Test one");

            Assert.IsTrue(data.Choices[0].Text == "Test choice one");
            Assert.IsTrue(data.Choices[1].Text == "Test choice two");
            Assert.IsTrue(data.Choices[2].Text == "Test choice three");

            Assert.IsTrue(data.Choices[0].Next == "End");
            Assert.IsTrue(data.Choices[1].Next == "End");
            Assert.IsTrue(data.Choices[2].Next == "End");

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
            DialogueScript script = new DialogueScript();

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
            BehaviourScript script = new BehaviourScript();

            ScriptException ex = Assert.Throws<ScriptException>(() => script.Parse(script_5));
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

                BehaviourScript script = new BehaviourScript();
                script.Parse(script_6);
                script.Interpreter.PreVisit();
                script.Interpreter.EnterBlockNode("Start");
                return ((DoubleValue)script.Interpreter.Current["d"]).Value;
            }

        }

        #endregion

        #region Test 7
        private string script_7 = @"
               
            [Start]
                {Text = 'Test one'}
                {Mood = 'Test Mood'}
                {Actor = 'Test Actor'}
                    {Choice = 'Test choice one' => 'End'};

            end

            [End]
                 {Text = 'Test one' => 'Start'}; 
            end
        ";

        [Test]
        public void Test7()
        {
            DialogueScript script = new DialogueScript();

            script.Parse(script_7);
            script.Interpreter.PreVisit();

            DialogueData data = (DialogueData)script.Interpreter.EnterBlockNode("Start");

            Assert.IsTrue(data.Text == "Test one");
            Assert.IsTrue(data.Mood == "Test Mood");
            Assert.IsTrue(data.ActorName == "Test Actor");

            Assert.Pass();
        }

        #endregion

        #region Test 8
        private string script_8 = @"
               
            [End]
                 {Text = 'Test one' => end }; 
            end
        ";

        [Test]
        public void Test_DialogueTermination()
        {
            DialogueScript script = new DialogueScript();

            script.Parse(script_8);
            script.Interpreter.PreVisit();

            script.Interpreter.EnterBlockNode("End");

            Assert.Pass();
        }

        #endregion

        #region Test 9

        private string script_9 = @"
               
            double d = 20;
            
            [Start]
                {Text = 'Start'}
                    {Choice = 'Choice1' => 'Choice1'}
                    {Choice = 'Choice2' => 'Choice2'};
            end
            
            [Choice1]  
                {Text = 'Choice1'}
                    {Choice = 'Start' => 'Start'}
                    {Choice = 'Choice2' => 'Choice2'};
            end

            [Choice2]
                {Text = 'Choice2'}
                    {Choice = 'Start' => 'Start'}
                    {Choice = 'Choice1' => end};
            end
        ";

        [Test]
        public void Test_DialogueHandler()
        {

            DialogueTestListener listener = new DialogueTestListener();
            DialogueTestHandler handler = new DialogueTestHandler();

            listener.Register(handler);

            handler.ReadScript(script_9);

            Assert.IsTrue(listener.IsActive == false);
            handler.Start();
            Assert.IsTrue(listener.IsActive == true);

            Assert.IsTrue(listener.Text == "Start");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Choice1");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice2");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Choice1");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice2");

            handler.ChooseOption(0);

            Assert.IsTrue(handler.CurrentData.Text == "Choice1");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Start");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice2");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Start");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice2");

            handler.ChooseOption(1);

            Assert.IsTrue(handler.CurrentData.Text == "Choice2");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Start");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice1");

            Assert.IsTrue(listener.ChoiceTexts[0] == "Start");
            Assert.IsTrue(listener.ChoiceTexts[1] == "Choice1");

            handler.ChooseOption(1);

            Assert.True(listener.IsActive == false);
            Assert.True(listener.Text == null);
            Assert.True(listener.ChoiceTexts.Count == 0);

            Assert.Pass();
        }

        #endregion
    }
}