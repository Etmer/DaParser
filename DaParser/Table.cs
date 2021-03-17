using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public class Table
    {
        private Dictionary<string, TableSymbol> symbols = new Dictionary<string, TableSymbol>();

        public object this[string index]
        {
            set { CreateTableValueFromObject(index, value); }
            get { return symbols[index]; }
        }

        public bool ContainsKey(string key) 
        {
            return symbols.ContainsKey(key);
        }
        public void Add(string key, TableSymbol value)
        {
            symbols.Add(key, value);
        }

        private void CreateTableValueFromObject(string index, object obj)
        {
            FunctionSymbol function = new FunctionSymbol(obj);
            symbols.Add(index, function);
        }
    }
}
