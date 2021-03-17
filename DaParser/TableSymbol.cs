using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DaScript
{
    public delegate object Function(params object[] arguments); 
    public enum Category
    {
        Variable,
        Number,
        Boolean,
        String,
        Function,
    }

    public class TableSymbol
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public virtual Category Cat { get; private set; }
        public virtual object Value { get; private set; }
        public TableSymbol(object value) { Value = value; }
    }

    public class Variable : TableSymbol
    {
        public Variable(object value) : base(value) { }
    }

    public class StringSymbol : BuiltInType 
    {
        public override Category Cat { get { return Category.String; } }

        public StringSymbol(object value) : base(value) { }
    }
    public class IntegerSymbol : BuiltInType
    {
        public override Category Cat { get { return Category.Number; } }
        public IntegerSymbol(object value) : base(value) { }
    }

    public class BuiltInType : TableSymbol
    {
        public BuiltInType(object value) : base(value) { }
    }

    public class FunctionSymbol : TableSymbol
    {
        private Delegate functionCall;
        private object functionOwner;
        private List<object> parameters = new List<object>();
        public FunctionSymbol(object value) : base(value)
        {
            CreateFromDelegate((Delegate)value);
        }
        public void Call()
        {
            functionCall.DynamicInvoke(parameters.ToArray());
        }

        private void CreateFromDelegate(Delegate d)
        {
            MethodInfo method = d.Method;
            parameters.Add(d.Target);

            ParameterExpression instance = Expression.Parameter(method.DeclaringType, "Instance");
            ParameterExpression[] parameter = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();
            List<ParameterExpression> allParameters = new List<ParameterExpression>() { instance };
            allParameters.AddRange(parameter);

            LambdaExpression lambda = Expression.Lambda(
                Expression.Call(instance, method, parameter),
                allParameters.ToArray());

            functionCall = lambda.Compile();
        }
    }

    public class EventSymbol : TableSymbol
    {
        public BlockNode Block { get { return Value as BlockNode; } }
        public EventSymbol(BlockNode value) : base(value) { }
    }
}
