using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class EventInterpreter : Interpreter
    {
        public EventInterpreter(Node node) : base(node) 
        {
            Globals["CallMeMaybe"] = (System.Action)CallMeMaybe;
        }

        public void VisitBlock(string name) 
        {
            EnterBlockNode(name);
        }

        private void CallMeMaybe() 
        {
            Console.WriteLine("Me writy");
        }
    }
}
