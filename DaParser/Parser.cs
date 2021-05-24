﻿using EventScript.Interfaces;
using EventScript.Literals;
using EventScript.Utils;
using System.Collections.Generic;

namespace EventScript
{
    public class Parser : ErrorRaiser
    {
        private Token currentToken;
        private Lexer lexer = null;

        public Code Parse(Lexer lexer)
        {
            this.lexer = lexer;
            return Consume_Program();
        }


        public Code Consume_Program() 
        {
            currentToken = lexer.GetNextToken();
            BlockStatement block = new BlockStatement();
            //Consume(TokenType.DIALOGUESCRIPT);

            while (currentToken.Type != TokenType.EOF)
            {
                block.Append(Consume_ProgramBlock());
            }
            
            Consume(TokenType.EOF);

            Code code = new Code();
            code.SetBlockStatement(block);

            return code;
        }

        public BlockStatement Consume_BlockStatement() 
        { 
            BlockStatement block = new BlockStatement();
            block.AppendRange(Consume_StatementList());

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

                    StringLiteral lit = ExpressionFactory.CreateStringLiteral(currentToken.Value.ToString(), currentToken);
                    BlockVariable blockVar = new BlockVariable(lit);

                    Consume(TokenType.ID);
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

             while (currentToken.Type != TokenType.END && currentToken.Type != TokenType.ELSE && currentToken.Type != TokenType.ELSEIF)
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
                    FunctionCallExpression call = Consume_FunctionCall();
                    Consume(TokenType.SEMI);
                    return call;

                case TokenType.CONDITION:
                    return Consume_ConditionalExpression();

                case TokenType.MEMBERDELIMITER_LEFT:
                    return Consume_DialogueExpression();
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

            Consume(TokenType.END);
            return blockList;
        }

        private ConditionBlock Consume_ConditionBlock()
        {
            switch (currentToken.Type) 
            {
                case TokenType.CONDITION:
                    return CreateBlock(0);

                case TokenType.ELSEIF:
                    return CreateBlock(1);

                case TokenType.ELSE:
                    Consume(TokenType.ELSE);
                    return ExpressionFactory.CreateConditionalBlock(2, new TRUE_Expression(), Consume_BlockStatement(), currentToken);
            }

            ConditionBlock CreateBlock(int precedence) 
            {
                Consume(TokenType.CONDITION, TokenType.ELSEIF);
                IExpression expr = Consume_Expression();
                Consume(TokenType.THEN);
                BlockStatement blockStmt = Consume_BlockStatement();
                return ExpressionFactory.CreateConditionalBlock(0, expr, blockStmt, currentToken);
            }

            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, currentToken);
        }

        private ConditionalExpression Consume_ConditionalExpression() 
        {
            return ExpressionFactory.CreateConditionalExpression(Consume_ConditionBlockList(), currentToken);
        }

        private DeclarationStatement Consume_VariableDeclaration() 
        {

            DeclarationStatement declStmt = new DeclarationStatement();
            string variableType = currentToken.Value.ToString();
            Consume(TokenType.TYPESPEC);
            string variableName = currentToken.Value.ToString();

            declStmt.SetType(variableType);
            declStmt.SetName(variableName);

            Consume(TokenType.ID);
            Consume(TokenType.ASSIGN);
            IExpression expression = Consume_Expression();

            declStmt.SetExpression(expression);

            Consume(TokenType.SEMI);

            return declStmt;
        }

        private IExpression Consume_Expression()
        {
            IExpression result = Consume_Term();

            while (currentToken.Type == TokenType.PLUS || currentToken.Type == TokenType.MINUS)
            {
                Token token = currentToken;
                Consume(TokenType.PLUS, TokenType.MINUS);

                result = ExpressionFactory.CreateBinary(result, Consume_Term(), token.Type, currentToken);
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
                term = ExpressionFactory.CreateBinary(term,Consume_Factor(), token.Type, token);
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
                    return ExpressionFactory.CreateUnary(Consume_Expression(), token.Type, token);

                case TokenType.NUMBER:
                    Consume(TokenType.NUMBER);
                    return ExpressionFactory.CreateNumberLiteral(token.Value.ToString(), token);

                case TokenType.L_PAREN:
                    Consume(TokenType.L_PAREN);
                    IExpression expr = Consume_Expression();
                    Consume(TokenType.R_PAREN);
                    return expr;

                case TokenType.ID:
                    return Consume_Variable();

                case TokenType.STRING:
                    Consume(TokenType.STRING);
                    return ExpressionFactory.CreateStringLiteral(token.Value.ToString(), token);

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
                    return ExpressionFactory.CreateAssignStatement(variable, Consume_Expression(), token);
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

        #region Dialogue

        private DialogueExpression Consume_DialogueExpression()
        {
            return ExpressionFactory.CreateDialogueExpression(Consume_TextMemberExpression(), Consume_ChoiceList(), currentToken);
        }

        private List<ChoiceMemberExpression> Consume_ChoiceList()
        {
         
            List<ChoiceMemberExpression> result = new List<ChoiceMemberExpression>();

            while (currentToken.Type != TokenType.SEMI)
            {
                result.Add(Consume_ChoiceExpression());
            }

            Consume(TokenType.SEMI);
            return result;
        }
        private TextMemberExpression Consume_TextMemberExpression()
        {
            Consume(TokenType.MEMBERDELIMITER_LEFT);
            Consume(TokenType.TEXT_MEMBER);
            Consume(TokenType.ASSIGN);

            IExpression text = Consume_Factor();

            Token token = currentToken;

            switch (token.Type) 
            {
                case TokenType.TRANSFER:
                    Consume(TokenType.TRANSFER);
                    IExpression next = Consume_Factor();
                    Consume(TokenType.MEMBERDELIMITER_RIGHT);
                    return ExpressionFactory.CreateTextMemberExpression(text, next, token);

                case TokenType.MEMBERDELIMITER_RIGHT:
                    Consume(TokenType.MEMBERDELIMITER_RIGHT);
                    return ExpressionFactory.CreateTextMemberExpression(text, null, token);
            }

            throw RaiseError(ScriptErrorCode.UNEXPECTED_TOKEN, currentToken);
        }

        private ChoiceMemberExpression Consume_ChoiceExpression() 
        {
            Token token = currentToken;
            IExpression condition = null;

             switch (token.Type) 
            {
                case TokenType.MEMBERDELIMITER_LEFT:
                    condition = new TRUE_Expression();
                    break;

                case TokenType.L_PAREN:
                    condition = Consume_Expression();
                    break;
            }

            Consume(TokenType.MEMBERDELIMITER_LEFT);
            Consume(TokenType.CHOICE_MEMBER);
            Consume(TokenType.ASSIGN);

            IExpression text = Consume_Factor();

            Consume(TokenType.TRANSFER);

            IExpression next = Consume_Factor();

            Consume(TokenType.MEMBERDELIMITER_RIGHT);

            return ExpressionFactory.CreateChoiceMemberExpression(condition, text, next, token);
        }

        #endregion

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
