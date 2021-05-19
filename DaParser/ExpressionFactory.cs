using EventScript.Interfaces;
using EventScript.Literals;
using System.Collections.Generic;

namespace EventScript.Utils
{
    public class ExpressionFactory
    {
        public static DialogueExpression CreateDialogueExpression(TextMemberExpression text, List<ChoiceMemberExpression> choiceList) 
        {
            DialogueExpression expr = new DialogueExpression();

            expr.AddChoiceExpression(choiceList);
            expr.SetTextExpression(text);

            return expr;
        }
        public static TextMemberExpression CreateTextMemberExpression(IExpression textLiteral, IExpression nextLiteral)
        {
            TextMemberExpression expr = new TextMemberExpression();

            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);

            return expr;
        }
        public static ChoiceMemberExpression CreateChoiceMemberExpression(IExpression condition, IExpression textLiteral, IExpression nextLiteral)
        {
            ChoiceMemberExpression expr = new ChoiceMemberExpression();

            expr.SetCondition(condition);
            expr.SetText(textLiteral);
            expr.SetNext(nextLiteral);

            return expr;
        }
        public static BlockStatement CreateBlockStatement(List<IExpression> statements) 
        {
            BlockStatement result = new BlockStatement();

            foreach (IExpression statement in statements) 
                result.Append(statement);

            return result;
        }

        public static BinaryExpression CreateBinary(IExpression left, IExpression right, TokenType operatorType) 
        {
            BinaryExpression result = new BinaryExpression();
            result.SetLeft(left);
            result.SetRight(right);
            result.SetOperatorType(operatorType);

            return result;
        }

        public static UnaryExpression CreateUnary(IExpression expression, TokenType operatorType)
        {
            UnaryExpression result = new UnaryExpression();
            result.SetExpression(expression);
            result.SetOperatorType(operatorType);

            return result;
        }

        public static AssignStatement CreateAssignStatement(Variable variable, IExpression expression) 
        {
            AssignStatement result = new AssignStatement();
            result.SetVariable(variable);
            result.SetExpression(expression);

            return result;
        }

        public static ConditionalExpression CreateConditionalExpression(List<ConditionBlock> conditionBlocks) 
        {
            ConditionalExpression result = new ConditionalExpression();

            result.AddConditionBlocks(conditionBlocks);
            return result;
        }

        public static ConditionBlock CreateConditionalBlock(IExpression expression, BlockStatement blockStmt)
        {
            ConditionBlock result = new ConditionBlock();

            result.SetCondition(expression);
            result.SetBlockStatement(blockStmt);
            return result;
        }

        public static Literal CreateStringLiteral(string value) 
        {
            return new Literal(value);
        }

        public static Literal CreateNumberLiteral(string value) 
        {
            return new Literal(value);
        }

        public static Literal CreateBooleanLiteral(string value)
        {
            return new Literal(value);
        }
    }
}
