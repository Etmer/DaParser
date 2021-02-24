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

        private Dictionary<string, List<TokenType>> expectedTokens = new Dictionary<string, List<TokenType>>()
        {
            { "TERM", new List<TokenType>(){ TokenType.MINUS, TokenType.PLUS} },
            { "FACTOR", new List<TokenType>(){ TokenType.MUL, TokenType.DIV} },
        };

        public Parser(Lexer lexer) 
        {
            this.lexer = lexer;
        }

        public Node Parse() 
        {
            ConsumeEntryToken();
            return Expression();
        }

        private Node Expression()
        {
            Node node = ConsumeTerm();

            while (expectedTokens["TERM"].Contains(currentToken.Type))
            {
                Token token = currentToken;

                if (currentToken.Type == TokenType.PLUS)
                {
                    Consume(TokenType.PLUS);
                }
                if (currentToken.Type == TokenType.MINUS)
                {
                    Consume(TokenType.MINUS);
                }

                node = new BinaryOpNode(token, node, ConsumeTerm());
            }
            return node;
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

        private Node ConsumeFactor() 
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return new NumberNode(token);
                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    Node result = Expression();
                    Consume(TokenType.R_PAREN);
                    return result;
            }
            throw new System.Exception("Syntax Error");
        }

        private Node ConsumeTerm()
        {
            Node node = ConsumeFactor();

            Token token = currentToken;
            while (expectedTokens["FACTOR"].Contains(currentToken.Type))
            {
                switch (token.Type)
                {
                    case TokenType.MUL:
                        Consume(TokenType.MUL);
                        break;
                    case TokenType.DIV:
                        Consume(TokenType.DIV);
                        break;
                }
                node = new BinaryOpNode(token, node, ConsumeFactor());
            }
            return node;
        }
    }
}
