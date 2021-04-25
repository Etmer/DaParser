using System;
using System.Collections.Generic;

namespace DaScript
{
    class Program
    {
        private static Script<DialogueInterpreter> script = new Script<DialogueInterpreter>();

        static string s =
@"        program

                [Start]
                    {Text} = 'Hello' => 
                        {Choice} = 'Good day Sir!' => 'Default'
                        {Choice} = 'Go die in a Pit!' => 'Unfriendly';
                end

                [Default]
                    {Text} = 'What can I do for you?' => 
                        {Choice} = 'I want to buy potions' => 'Buy'
                        {Choice} = 'I want to sell potions' => 'Sell'
                        {Choice} = 'Goodbye for now!' => 'End';
                end


                [Sell]
                    {Text} = 'I will have a look at your wares!' => 
                        {Choice} = 'I changed my mind since I do not have potions' => 'Default';
                end  
                
                [Buy]
                    {Text} = 'Have a look!' => 
                        {Choice} = 'Health Potion' => 'Default'
                        {Choice} = 'Mana Potion' => 'Default';
                end  

                [Unfriendly]
                    {Text} = 'Go away' => end;
                end    
                
                [End]
                    {Text} = 'Goodbye' => end;
                end  
        ";


        static void Main(string[] args)
        {
            ProcessSourcestring(s);
        }

        static void ProcessSourcestring(string input)
        {
            script.Interpreter.OnStart += Print;
            script.Interpreter.OnUpdate += Print;

            script.Parse(input);
            script.Interpreter.Start();
        }

        static void Print(string text, List<DialogueOption> choices) 
        {
            Console.WriteLine($"Text: {text}");

            foreach (DialogueOption choice in choices)
            {
                if(choice.HasInfo)
                    Console.WriteLine($"Choice: {choice.Text}");
            }
        }

    }
    
}
