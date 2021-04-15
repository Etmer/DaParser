using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public enum ScriptErrorCode
    {
        UNEXPECTED_TOKEN,
        ID_NOT_FOUND,
        ID_ALREADY_DECLARED
    }

    public class ScriptErrorHandler
    {
        public System.Action<string> OnError;
        public bool HasErrors { get; private set; } = false;

        public void RaiseError(ScriptErrorCode errorCode, Token token) 
        {
            string message = "Unknown Error";
            HasErrors = true;


            OnError?.Invoke(message);
        }
    }
}
