using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Dynamic;

namespace DLR_Compiler
{
    class Environment
    {
        Environment parent;
        Dictionary<String, ObjBox> env;

        public Environment()
        {
            parent = null;
            env = new Dictionary<String, ObjBox>();
        }

        public Environment(Environment _parent)
        {
            parent = _parent;
            env = new Dictionary<String, ObjBox>();
        }

        public dynamic lookup(String name)
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

        public Type lookupType(String name)
        {
            if (env.ContainsKey(name))
            {
                return env[name].getType();
            }
            if (parent == null)
            {
                throw new RuntimeException("variable reference to unscoped variable: " + name.ToString());
            }
            else
            {
                return parent.lookupType(name);
            }
        }

        public void add(String name, ObjBox value)
        {
            env.Add(name, value);
        }
    }

   public class FunctionHolder
    {
        int count;
        public Delegate func;
        public FunctionHolder(Delegate _func)
        {
            func = _func;
            count = 0;
        }

        public dynamic invoke(List<Object> arguments)
        {
            count += 1;
            //Console.WriteLine("Got called");
            Console.WriteLine("Count is: " + count.ToString());
            return func.DynamicInvoke(arguments.ToArray());
        }
    }

    class ObjBox
    {
        private Object obj ;
        private Type type;
        public System.Reflection.MethodInfo converter;
        
        public ObjBox(Object _obj, Type _type)
        {
            this.obj = _obj;
            this.type = _type;
            converter = typeof(TypeUtils).GetMethod("cast").MakeGenericMethod(_type);
        }

        public Object getObj()
        {
            return this.obj;
        }

        public System.Reflection.MethodInfo getConv()
        {
            return this.converter;
        }

        public Type getType()
        {
            return this.type;
        }
    }
}
