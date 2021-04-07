using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DaScript
{
    public delegate object Function(params object[] arguments); 
  
    public interface ITableValue
    {  
        object GetValue();
    }
    public abstract class TableValue<T> : ITableValue
    {
        protected T Value;


        public object GetValue() 
        {
            return Value;
        }
        public override bool Equals(object obj)
        {
            ITableValue symbol = obj as ITableValue;

            if (symbol != null)
                return Value.Equals(symbol.GetValue());

            return Value.Equals(obj);
        }
    }


    public class StringValue : BuiltInType<string>
    {
        public StringValue() : base() { }
        public StringValue(object value) : base(value) { }
    }
    public class DoubleValue : BuiltInType<double>
    {
        public DoubleValue() : base() { }
        public DoubleValue(object value) : base(value) { }
    }
    public class IntegerValue : BuiltInType<int>
    {
        public IntegerValue() : base() { }
        public IntegerValue(object value) : base(value) { }
    }
    public class BooleanValue : BuiltInType<bool>
    {
        public BooleanValue() : base() { }
        public BooleanValue(object value) : base(value) { }
    }
    public class BuiltInType<T> : TableValue<T>
    {
        public BuiltInType() : base() { }
        public BuiltInType(object value) 
        {
            Value = (T)value;
        }
    }

    public class FunctionValue : TableValue<Delegate>
    {
        private object functionOwner;
        public FunctionValue(object value)
        {
            CreateFromDelegate((Delegate)value);
        }
        public object Call(List<object> arguments)
        {
            List<object> actualArguments = new List<object>() { functionOwner };
            actualArguments.AddRange(arguments);
            return Value.DynamicInvoke(actualArguments.ToArray());
        }

        private void CreateFromDelegate(Delegate d)
        {
            MethodInfo method = d.Method;
            functionOwner = d.Target;

            ParameterExpression instance = Expression.Parameter(method.DeclaringType, "Instance");
            ParameterExpression[] parameter = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();
            List<ParameterExpression> allParameters = new List<ParameterExpression>() { instance };
            allParameters.AddRange(parameter);

            LambdaExpression lambda = Expression.Lambda(
                Expression.Call(instance, method, parameter),
                allParameters.ToArray());

            Value = lambda.Compile();
        }
    }

    public class EventValue : TableValue<BlockNode>
    {
        public EventValue(BlockNode value) 
        {
            Value = value;
        }
    }
}
