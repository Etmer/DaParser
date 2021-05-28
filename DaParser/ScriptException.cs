using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventScript
{
    public class ScriptException : Exception 
    {
        public ScriptException(string message) : base(message) { }
        public ScriptException() : base() { }
    }
}
