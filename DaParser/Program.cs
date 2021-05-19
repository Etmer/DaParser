using EventScript;
using System;
using System.Collections.Generic;

namespace DaScript
{
    class Program
    {
        private static DialogueTester tester = new DialogueTester();

        static string s =
@"        dialogue
                
                double d = 100;
                
                [Start]
                    {Text = 'Hello'}
                        {Choice = 'Good *day* Sir!' => 'Default'}
                        {Choice = 'Go die in a Pit!' => 'Unfriendly'};
                end

                [Default]
                    {Text = 'What can I do for you?'}
                        (d < 90) {Choice = 'I want to buy potions' => 'Buy'}
                        {Choice = 'I want to sell potions' => 'Sell'}
                        {Choice = 'Goodbye for now!' => 'End'}
                        {Choice = 'Do the test!' => 'Test'};

                    if(d == 100) then
                        CallMyCall();
                    end
                end
                
                [Test]
                    {Text = 'I am a test and should lead to default' => 'Default'};

                    if(d == 100) then
                        d = 90;
                    elif(d == 25) then
                        d = 80;
                    end
                end


                [Sell]
                    {Text = 'I will have a look at your wares!'}
                        {Choice = 'I changed my mind since I do not have potions' => 'Default'};
                end  
                
                [Buy]
                    {Text = 'Have a look!'}
                        {Choice = 'Health Potion' => 'Default'}
                        {Choice = 'Mana Potion' => 'Default'};
                end  

                [Unfriendly]
                    {Text = 'Go away' => 'End'};
                end    
                
                [End]
                    {Text = 'Goodbye' => 'End'};
                end  
        ";


        static void Main(string[] args)
        {
            ProcessSourcestring(s);
        }

        static void ProcessSourcestring(string input)
        {
            tester.OnStart += Print;
            tester.OnUpdate += Print;

            tester.Start(input);
        }

        static void Print(string text, List<DialogueChoice> choices) 
        {
            Console.WriteLine($"Text: {text}");

            foreach (DialogueChoice choice in choices)
            {
                if(choice.HasInfo)
                    Console.WriteLine($"\t Choice: {choice.Text}");
            }
        }

    }
    
}
