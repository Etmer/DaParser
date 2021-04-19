using System;
using System.Collections.Generic;
using System.Text;
 
namespace DaScript
{
    public class Memory
    {
        private Memory parent;
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
            object value = null;

            if (obj is Delegate)
            {
                value = new FunctionValue(obj);
            }
            else if (obj is CompundStatementNode)
            {
                value = new BlockValue(obj as BlockNode);
            }
            else if(obj is int)
            {
                value = new IntegerValue(obj);
            }
            else if (obj is string)
            {
                value = new StringValue(obj);
            }
            else if (obj is double)
            {
                value = new DoubleValue(obj);
            }
            else if (obj is bool)
            {
                value = new BooleanValue(obj);
            }
            else
                value = obj;


            if (memory.ContainsKey(identifier))
                memory[identifier] = value;
            else
                memory.Add(identifier, value);
        }
    }
}
