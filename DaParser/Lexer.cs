using System.Collections.Generic;

namespace DaScript
{
    public class Lexer : InterpreterStep
    {
        private int index = 0;
        private Tokenizer tokenizer = new Tokenizer();
        private List<Token> tokens = new List<Token>();

        public void Tokenize(string input) 
        {
            tokens = tokenizer.Tokenize(input);
        }

        /// <summary>
        /// Returns true if the current inspected char from the input string has a value
        /// </summary>
        /// <returns></returns>
        public bool HasToken() 
        {
            return index < tokens.Count;
        }

        /// <summary>
        /// Increases the inspected index by 1 and returns the token at that index
        /// </summary>
        /// <returns></returns>
        public Token GetNextToken() 
        {
            return tokens[index++];
        }
    }
}
