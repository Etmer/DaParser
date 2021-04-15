namespace DaScript
{
    public class InterpreterStep
    {
        public event System.Action<ScriptErrorCode, Token> OnError;

        protected virtual void RaiseError(ScriptErrorCode errorCode, Token token) 
        {
            OnError?.Invoke(errorCode, token);
        }
    }
}
