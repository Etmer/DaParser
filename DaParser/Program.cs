using System;

namespace DaScript
{
    class Program
    {
        static string s =
@"        program

                double d = 1;
                [Start]
                    {Text} = 'Hello' => 'Wares';
                end

                [Wares]
                    {Text} = 'Go away' => 'Quest';
                end  
                
                [Quest]
                    GoTo('MyNextName');
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

        static void PrintError(string msg) 
        {
            Console.WriteLine($"======={msg}=======");
        }
    }
    
}
