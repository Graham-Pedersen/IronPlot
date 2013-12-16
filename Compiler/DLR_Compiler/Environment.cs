using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace DLR_Compiler
{
    class Environment
    {
        Environment parent;
        Dictionary<String, int> env;

        public Environment()
        {
            parent = null;
            env = new Dictionary<String, int>();
        }

        public Environment(Environment _parent)
        {
            parent = _parent;
            env = new Dictionary<String, int>();
        }

        public int lookup(String name)
        {
            if (env.ContainsKey(name))
            {
                return env[name];
            }
            if(parent == null)
            {
                throw new RuntimeException("variable reference to unscoped variable: " + name.ToString());
            }
            else
            {
                return parent.lookup(name);
            }
        }

        public void add(String name, int value)
        {
            env.Add(name, value);
        }
    }
}
