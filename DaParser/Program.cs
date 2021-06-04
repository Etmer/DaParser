using EventScript;
using System;
using System.Collections.Generic;

namespace DaScript
{
    class Program
    {
       static string s =
       @"
                double d = 10;

                [Start]
                    if(d == 10) then
                        d = 150;
                    elif(d < 10) then
                        d = 4;
                    elif(d == 20)
                        d = 5;
                    elif(d > 40) then
                        d = 6;
                    end
                end
            ";

        public static void Main(string[] args)
        {
            BehaviourScript script = new BehaviourScript();

            script.Parse(s);
            script.Interpreter.Visit();
            script.Interpreter.EnterBlockNode("Start");
        }
    }
    
}
