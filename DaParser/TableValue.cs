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
        object GetValue(List<object> arguments = null);
    }
    public abstract class TableValue<T> : ITableValue
    {
        protected T Value;

        public virtual object GetValue(List<object> arguments = null)
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

        public override object GetValue(List<object> arguments)
        {
            arguments.Insert(0, functionOwner);
            return Value.DynamicInvoke(arguments.ToArray());
        }
    }

    public class BlockValue : TableValue<BlockNode>
    {
        public BlockValue(BlockNode value) 
        {
            Value = value;
        }

        public BlockNode GetBlock() 
        {
            return Value;
        }
    }
}
