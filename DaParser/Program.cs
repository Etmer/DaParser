using System;

namespace DaScript
{
    class Program
    {
        static string s =
@"        program
               
                string s = 'TestString';
                int i = 1;
                double d = 2;
                bool b = GetMeMyBool();

                [Start]

                    SetText('Hello Adventurer');
                    SetChoice('Show me your wares!', 'Wares');

                    if(i =!= 2) then
                        SetChoice('I did not find this on the doorstep', 'Quest');
                    end

                    if(b) then
                        SetChoice('I found this on the doorstep', 'Quest');
                    end
                end

                [Wares]
                    SetText('I will not talk to you if you dont need anything');
                    SetChoice('I need Potions','MyNextName');
                    SetChoice('K Bye','Final');
                end  
                
                [Quest]
                    GoTo('MyNextName');
                end

                [MyNextName]
                    SetText('My treasured trousers!!! I am so relieved thank you');
                    SetChoice('I need Potions','Final');
                end  
                
                [Final]
                    SetText('Thank you!');
                end
        ";


        static void Main(string[] args)
        {
            ProcessSourcestring(s);
        }

        static void ProcessSourcestring(string input)
        {
            Script<DialogueInterpreter> script = new Script<DialogueInterpreter>();
            script.Parse(input);
            script.Interpreter.StartDialogue();

            while (true)
            {
                int idx = 0;
                if (int.TryParse(Console.ReadLine(), out idx))
                    script.Interpreter.UpdateDialogue(idx);
            }
        }
    }
    
}
