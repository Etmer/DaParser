using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
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
    }
}
