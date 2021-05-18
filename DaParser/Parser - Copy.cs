using EventScript.Interfaces;
using EventScript.Literals;
using EventScript.Utils;
using System.Collections.Generic;

namespace EventScript
{
    public class Parser : ErrorRaiser
    {
        private Token currentToken;
        private Lexer lexer = null;

        public BlockStatement Parse(Lexer lexer)
        {
            this.lexer = lexer;
            return Consume_Program();
        }


        public BlockStatement Consume_Program() 
        {
            currentToken = lexer.GetNextToken();
            BlockStatement program = new BlockStatement();
            Consume(TokenType.DIALOGUESCRIPT);

            while (currentToken.Type != TokenType.EOF)
            {
                program.Append(Consume_ProgramBlock());
            }
            
            Consume(TokenType.EOF);
            return program;
        }

        public BlockStatement Consume_BlockStatement() 
        { 
            currentToken = lexer.GetNextToken();
            BlockStatement block = new BlockStatement();
            block.AppendRange(Consume_StatementList());

            Consume(TokenType.END);
            return block;
        }

        private Statement Consume_ProgramBlock()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.TYPESPEC:
                    return Consume_VariableDeclaration();
                
                case TokenType.L_BLOCK:
                    return Consume_BlockDeclaration();
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private BlockVariable Consume_BlockDeclaration()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.L_BLOCK:
                    Consume(TokenType.L_BLOCK);
                    BlockVariable blockVar = new BlockVariable(token.Value.ToString());
                    Consume(TokenType.R_BLOCK);

                    blockVar.SetBlockStatement(Consume_BlockStatement());

                    Consume(TokenType.END);

                    return blockVar;
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private List<IExpression> Consume_StatementList()
        {
            IExpression stmt = Consume_Statement();

            List<IExpression> statementList = new List<IExpression>() { stmt };

             while (currentToken.Type != TokenType.END)
            {
                statementList.Add(Consume_Statement());
            }
            return statementList;
        }
        private IExpression Consume_Statement()
        {
            Token token = currentToken;
            switch (token.Type)
            {
                case TokenType.ID:
                    IExpression stmt = Consume_ID();
                    Consume(TokenType.SEMI);
                    return stmt;

                case TokenType.CALL:
                    //Node call = Consume_FunctionCall();
                    //Consume(TokenType.SEMI);
                    //return call;

                case TokenType.CONDITION:
                    Consume(TokenType.CONDITION);
                    return Consume_ConditionalExpression();

                case TokenType.MEMBERDELIMITER_LEFT:
                    //Node dialogueMember = Consume_DialogueMember();
                    //Consume(TokenType.SEMI);
                    //return dialogueMember;
                    break;
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }
        private List<ConditionBlock> Consume_ConditionBlockList()
        {
            List<ConditionBlock> blockList = new List<ConditionBlock>();

            do
            {
                blockList.Add(Consume_ConditionBlock());
            } while (currentToken.Type != TokenType.END);

            return blockList;
        }

        private ConditionBlock Consume_ConditionBlock()
        {
            switch (currentToken.Type) 
            {
                case TokenType.CONDITION:
                case TokenType.ELSEIF:
                    Consume(TokenType.CONDITION, TokenType.ELSEIF); 
                    return ExpressionFactory.CreateConditionalBlock(Consume_Expression(), Consume_BlockStatement());
                case TokenType.ELSE:
                    Consume(TokenType.ELSE);
                    return ExpressionFactory.CreateConditionalBlock(new TRUE_Expression(), Consume_BlockStatement());
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, currentToken);
        }

        private ConditionalExpression Consume_ConditionalExpression() 
        {
            return ExpressionFactory.CreateConditionalExpression(Consume_ConditionBlockList());
        }

        private DeclarationStatement Consume_VariableDeclaration() 
        {
            Consume(TokenType.TYPESPEC);

            DeclarationStatement declStmt = new DeclarationStatement();
            string variableName = currentToken.Value.ToString();

            declStmt.SetVariable(new Variable(variableName));

            Consume(TokenType.ASSIGN);
            IExpression expression = Consume_Expression();

            declStmt.SetExpression(expression);

            return declStmt;
        }

        private IExpression Consume_Expression()
        {
            IExpression result = Consume_Term();

            while (currentToken.Type == TokenType.PLUS || currentToken.Type == TokenType.MINUS)
            {
                Token token = currentToken;
                Consume(TokenType.PLUS, TokenType.MINUS);

                result = ExpressionFactory.CreateBinary(result, Consume_Term(), token.Type);
            }

            return result;
        }

        private IExpression Consume_Term()
        {
            IExpression term = Consume_Factor();

            Token token = currentToken;
            while (IsOperator())
            {
                switch (token.Type)
                {
                    case TokenType.MUL:
                        Consume(TokenType.MUL);
                        break;
                    case TokenType.DIV:
                        Consume(TokenType.DIV);
                        break;
                    case TokenType.EQUALS:
                        Consume(TokenType.EQUALS);
                        break;
                    case TokenType.SMALLER:
                        Consume(TokenType.SMALLER);
                        break;
                    case TokenType.GREATER:
                        Consume(TokenType.GREATER);
                        break;
                    case TokenType.SMALLEREQUALS:
                        Consume(TokenType.SMALLEREQUALS);
                        break;
                    case TokenType.GREATEREQUALS:
                        Consume(TokenType.GREATEREQUALS);
                        break;
                }
                term = ExpressionFactory.CreateBinary(term,Consume_Factor(), token.Type);
            }
            return term;
        }

        private IExpression Consume_Factor()
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    Consume(TokenType.MINUS, TokenType.PLUS);
                    return ExpressionFactory.CreateUnary(Consume_Expression(), token.Type);

                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return new Literal(token.Value);

                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    IExpression expr = Consume_Expression();
                    Consume(TokenType.R_PAREN);
                    return expr;

                case TokenType.ID:
                    return Consume_Variable();

                case TokenType.STRING:
                    Consume(TokenType.STRING);
                    return ExpressionFactory.CreateStringLiteral(token.Value.ToString());

                case TokenType.CALL:
                    return Consume_FunctionCall();

            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private IExpression Consume_ID()
        {
            Token token = currentToken;
            Variable variable = Consume_Variable();

            switch (currentToken.Type)
            {
                case TokenType.ASSIGN:
                    Consume(TokenType.ASSIGN);
                    return ExpressionFactory.CreateAssignStatement(variable, Consume_Expression());
            }
            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private FunctionCallExpression Consume_FunctionCall()
        {
            Token token = currentToken;

            FunctionCallExpression call = new FunctionCallExpression(token.Value.ToString());
            Consume(TokenType.CALL);
            Consume(TokenType.L_PAREN);

            while (currentToken.Type != TokenType.R_PAREN)
            {
                call.AddArgument(Consume_Term());
                if (currentToken.Type == TokenType.COMMA)
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.R_PAREN);
            return call;
        }

        private Variable Consume_Variable()
        {
            Token token = currentToken;
            Consume(TokenType.ID);

            return new Variable(token.Value as string);
        }

        #region HelperFunctions

        //Consumes a Token until it reaches an EOF token
        private void Consume(params TokenType[] expectedTypes)
        {
            Token token = currentToken;

            foreach (TokenType type in expectedTypes)
            {
                if (token.Type == type)
                {
                    if (currentToken.Type != TokenType.EOF)
                    {
                        currentToken = lexer.GetNextToken();
                        return;
                    }
                    return;
                }
            }

            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, token);
        }

        private bool IsOperator() 
        {
            return  currentToken.Type == TokenType.MUL ||
                    currentToken.Type == TokenType.DIV ||
                    currentToken.Type == TokenType.EQUALS ||
                    currentToken.Type == TokenType.GREATER ||
                    currentToken.Type == TokenType.GREATEREQUALS ||
                    currentToken.Type == TokenType.SMALLER ||
                    currentToken.Type == TokenType.SMALLEREQUALS;

        }

        #endregion
    }
}
