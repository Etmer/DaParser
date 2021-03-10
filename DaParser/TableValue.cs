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

    class TableValue
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public Category Cat { get; private set; }
        public object Value { get; private set; }
    }

    class Variable : TableValue
    {

    }

    class BuiltInType : TableValue
    {

    }

    class FunctionValue : TableValue
    {
        private Delegate functionCall;
        private object functionOwner;
        private List<object> parameters = new List<object>();
        public void Call()
        {
            functionCall.DynamicInvoke(parameters.ToArray());
        }

        public void CreateFromDelegate(Delegate d)
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
}
