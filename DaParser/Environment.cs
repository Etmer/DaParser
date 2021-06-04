using System;
using System.Collections.Generic;
using System.Text;
 
namespace EventScript
{
    public class Environment
    {
        private Environment parent;
        private Dictionary<string, IValue> memory = new Dictionary<string, IValue>();


        public Environment() { }
        public Environment(Environment parent) { this.parent = parent; }

        public object this[string index]
        {
            set { CreateTableValueFromObject(index, value); }
            get { return GetTableValue(index); }
        }

        public void Add(string key, IValue value)
        {
            memory.Add(key, value);
        }

        private Environment GetEnvironmentForKey(string key)
        {
            if (memory.ContainsKey(key))
                return this;
            else if (parent != null)
                return parent.GetEnvironmentForKey(key);

            return null;
        }

        private IValue GetTableValue(string key) 
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
            IValue value = null;

            if (obj is Delegate)
                value = new FunctionValue(obj);
            else if (obj is BlockValue)
                value = obj as BlockValue;
            else if(obj is int)
                value = new IntegerValue(obj);
            else if (obj is string)
                value = new StringValue(obj);
            else if (obj is double)
                value = new DoubleValue(obj);
            else if (obj is bool)
                value = new BooleanValue(obj);
            else if(obj is DialogueData)
                value = obj as DialogueData;


            Environment env = GetEnvironmentForKey(identifier);

            if (env == null)
                env = this;

            if (env.memory.ContainsKey(identifier))
                env.memory[identifier] = value;
            else
                env.memory.Add(identifier, value);
        }

    }
}
