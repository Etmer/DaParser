using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    class Parser
    {
        private Token currentToken;
        private Token lastToken;
        private Lexer lexer = null;
        private bool process = true;

        private double result;

        private Dictionary<string, List<TokenType>> expectedTokens = new Dictionary<string, List<TokenType>>()
        {
            { "TERM", new List<TokenType>(){ TokenType.MINUS, TokenType.PLUS} },
            { "FACTOR", new List<TokenType>(){ TokenType.MUL, TokenType.DIV} },
        };

        public Parser(Lexer lexer) 
        {
            this.lexer = lexer;
        }

        public void Parse() 
        {
            if (ConsumeEntryToken())
            {
                
                while (process)
                {
                    Expression();
                }
                Console.WriteLine(result.ToString());
            }
        }

        private void Expression()
        {
            ConsumeTerm();

            while (expectedTokens["TERM"].Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.PLUS)
                {
                    Consume(TokenType.PLUS);
                    result += ConsumeTerm();
                }
                if (currentToken.Type == TokenType.MINUS)
                {
                    Consume(TokenType.MINUS);
                    result -= ConsumeTerm();
                }
            }
        }

        private bool Consume(params TokenType[] expectedTypes)
        {
            foreach (TokenType type in expectedTypes)
            {
                if (currentToken.Type == type)
                {
                    if (lexer.HasToken())
                    {
                        lastToken = currentToken;
                        currentToken = lexer.GetNextToken();
                        return true;
                    }
                    else
                    {
                        process = false;
                        return false;
                    }
                }
            }
            throw new System.Exception("Syntax Error");
        }

        private bool ConsumeEntryToken() 
        {
            Token token = lexer.GetNextToken();

            currentToken = token;
            lastToken = token;

            return true;
        }

        private double ConsumeFactor() 
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return token.GetValue<double>();
                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    Expression();
                    Consume(TokenType.R_PAREN);
                    break;
            }
            throw new System.Exception();
        }

        private double ConsumeTerm()
        {
            result = ConsumeFactor();

            Token token = currentToken;
            while (expectedTokens["FACTOR"].Contains(currentToken.Type))
            {
                switch (token.Type)
                {
                    case TokenType.MUL:
                        Consume(TokenType.MUL);
                        result *= ConsumeFactor();
                        break;
                    case TokenType.DIV:
                        Consume(TokenType.DIV);
                        result /= ConsumeFactor();
                        break;
                }
            }
            return result;
        }
    }
}
