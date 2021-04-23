using System;

namespace DaScript
{
    public class ErrorRaiser
    {
        public enum ScriptErrorCode
        {
            UNEXPECTED_TOKEN,
            ID_NOT_FOUND,
            ID_ALREADY_DECLARED,
            UNDEFINED_SYMBOL
        }

        protected virtual Exception RaiseError(ScriptErrorCode errorCode, Token token) 
        {
            string message = "Unknown Error";

            switch (errorCode)
            {
                case ScriptErrorCode.ID_NOT_FOUND:
                    message = $"Semantic Error: Undeclared ID: {token.GetValue()} at {token.Line}.{token.Column}";
                    break;
                case ScriptErrorCode.ID_ALREADY_DECLARED:
                    message = $"Semantic Error: ID aleady declared: {token.GetValue()} at {token.Line}.{token.Column}";
                    break;
                case ScriptErrorCode.UNEXPECTED_TOKEN:
                    message = $"Parsing Error: Unexpected Token: {token.Type} at {token.Line}.{token.Column}";
                    break;
                case ScriptErrorCode.UNDEFINED_SYMBOL:
                    message = $"Semantic Error: Undefined symbol: {token.Type} at {token.Line}.{token.Column}";
                    break;
            }
            return new System.Exception(message);
        }
    }
}
