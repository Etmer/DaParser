using System;
using System.Collections.Generic;
using System.Text;
 
namespace DaScript
{
    public class Table
    {
        private Table parent;
        private Dictionary<string, object> memory = new Dictionary<string, object>();

        public object this[string index]
        {
            set { CreateTableValueFromObject(index, value); }
            get { return GetTableValue(index); }
        }

        public void Add(string key, ITableValue value)
        {
            memory.Add(key, value);
        }

        private bool ContainsKey(string key) 
        {
            if (!memory.ContainsKey(key))
                return parent.ContainsKey(key);

            return true;
        }

        private object GetTableValue(string key) 
        {
            if (memory.ContainsKey(key))
                return memory[key];
            else if (parent != null)
                return parent.GetTableValue(key);
            else
                throw new System.Exception($"No Table Value found for key: {key}");
        }

        private void CreateTableValueFromObject(string identifier, object obj)
        {
            ITableValue symbol = null;

            if (obj is Delegate)
            {
                symbol = new FunctionValue(obj);
            }
            if(obj is int)
            {
                symbol = new IntegerValue(obj);
            }
            if (obj is string)
            {
                symbol = new StringValue(obj);
            }
            if (obj is double)
            {
                symbol = new DoubleValue(obj);
            }

            if (memory.ContainsKey(identifier))
                memory[identifier] = symbol;
            else
                memory.Add(identifier, symbol);
        }
    }
}
