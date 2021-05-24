using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript.Utils
{
    public class ExpressionFactory
    {
        public static DialogueExpression CreateDialogueExpression(TextMemberExpression text, List<ChoiceMemberExpression> choiceList, Token token) 
        {
            DialogueExpression expr = new DialogueExpression();

            expr.AddChoiceExpression(choiceList);
            expr.SetTextExpression(text);
            expr.SetToken(token);

            return expr;
        }
        public static TextMemberExpression CreateTextMemberExpression(IExpression textLiteral, IExpression nextLiteral, Token token)
        {
            TextMemberExpression expr = new TextMemberExpression();

            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);
            expr.SetToken(token);

            return expr;
        }
        public static ChoiceMemberExpression CreateChoiceMemberExpression(IExpression condition, IExpression textLiteral, IExpression nextLiteral, Token token)
        {
            ChoiceMemberExpression expr = new ChoiceMemberExpression();

            expr.SetCondition(condition);
            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);
            expr.SetToken(token);

            return expr;
        }
        public static BlockStatement CreateBlockStatement(List<IExpression> statements, Token token) 
        {
            BlockStatement result = new BlockStatement();
            result.SetToken(token);

            foreach (IExpression statement in statements) 
                result.Append(statement);

            return result;
        }

        public static BinaryExpression CreateBinary(IExpression left, IExpression right, TokenType operatorType, Token token) 
        {
            BinaryExpression result = new BinaryExpression();
            result.SetLeft(left);
            result.SetRight(right);
            result.SetOperatorType(operatorType);
            result.SetToken(token);

            return result;
        }

        public static UnaryExpression CreateUnary(IExpression expression, TokenType operatorType, Token token)
        {
            UnaryExpression result = new UnaryExpression();
            result.SetExpression(expression);
            result.SetOperatorType(operatorType);
            result.SetToken(token);

            return result;
        }

        public static AssignStatement CreateAssignStatement(Variable variable, IExpression expression, Token token) 
        {
            AssignStatement result = new AssignStatement();
            result.SetVariable(variable);
            result.SetExpression(expression);
            result.SetToken(token);

            return result;
        }

        public static ConditionalExpression CreateConditionalExpression(List<ConditionBlock> conditionBlocks, Token token) 
        {
            ConditionalExpression result = new ConditionalExpression();
            result.SetToken(token);

            result.AddConditionBlocks(conditionBlocks);
            return result;
        }

        public static ConditionBlock CreateConditionalBlock(int precedence, IExpression expression, BlockStatement blockStmt, Token token)
        {
            ConditionBlock result = new ConditionBlock();

            result.SetCondition(expression);
            result.SetBlockStatement(blockStmt);
            result.SetPrecedence(precedence);
            result.SetToken(token);

            return result;
        }

        public static StringLiteral CreateStringLiteral(string value, Token token) 
        {
            StringLiteral lit = new StringLiteral(value);
            lit.SetToken(token);
            return lit;
        }

        public static NumberLiteral CreateNumberLiteral(string value, Token token) 
        {
            NumberLiteral lit = new NumberLiteral(value);
            lit.SetToken(token);
            return lit;
        }

        public static BooleanLiteral CreateBooleanLiteral(string value, Token token)
        {
            BooleanLiteral lit = new BooleanLiteral(value);
            lit.SetToken(token);
            return lit;
        }
    }
}
