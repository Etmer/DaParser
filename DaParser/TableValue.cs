using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EventScript
{
    public delegate object Function(params object[] arguments); 
  
    public interface IValue
    {  
        object GetValue(List<object> arguments = null);
    }
    public abstract class TableValue<T> : IValue
    {
        public T Value { get; protected set; }

        public virtual object GetValue(List<object> arguments = null)
        {
            return Value;
        }
    }

    public abstract class PrimitiveValue<T> : TableValue<T> where T : IComparable 
    {
        public static bool operator <(PrimitiveValue<T> lhs, PrimitiveValue<T> rhs)
        {
            return lhs.Value.CompareTo(rhs.Value) == 1;
        }
        public static bool operator >(PrimitiveValue<T> lhs, PrimitiveValue<T> rhs) 
        {
            return lhs.Value.CompareTo(rhs.Value) == 1;
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
    public class BuiltInType<T> : PrimitiveValue<T> where T : IComparable
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

            ParameterExpression instance = System.Linq.Expressions.Expression.Parameter(method.DeclaringType, "Instance");
            ParameterExpression[] parameter = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();
            List<ParameterExpression> allParameters = new List<ParameterExpression>() { instance };
            allParameters.AddRange(parameter);

            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(
                System.Linq.Expressions.Expression.Call(instance, method, parameter),
                allParameters.ToArray());

            Value = lambda.Compile();
        }

        public override object GetValue(List<object> arguments)
        {
            arguments.Insert(0, functionOwner);
            return Value.DynamicInvoke(arguments.ToArray());
        }
    }

    public class BlockValue : TableValue<BlockStatement>
    {
        public BlockValue(BlockStatement value) 
        {
            Value = value;
        }

    }
}
