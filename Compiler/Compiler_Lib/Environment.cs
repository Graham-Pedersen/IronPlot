using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Dynamic;
using System.Numerics;

namespace CompilerLib
{
    public class Environment
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

        public Boolean check(String name)
        {
            if (env.ContainsKey(name))
            {
                return true;
            }
            if (parent == null)
            {
                return false;
            }
            else
            {
                return parent.check(name);
            }
        }

        public ObjBox lookup(String name)
        {
            if (env.ContainsKey(name))
            {
                return env[name];
            }
            if (parent == null)
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

        public void set(String name, ObjBox newValue)
        {
            if (env.ContainsKey(name))
            {
                env[name] = newValue;
            }
            else
            {
                if (parent == null)
                {
                    throw new RuntimeException("variable reference to unscoped variable: " + name.ToString());
                }
                parent.set(name, newValue);
            }
            
        }
    }

    public class FunctionHolder
    {
        //int count;
        public Delegate func;
        public int param_num;
        public FunctionHolder(Delegate _func, int count)
        {
            func = _func;
            param_num = count;
            //count = 0;
        }

        public ObjBox invoke(List<Object> arguments)
        {
            //count += 1;
            //Console.WriteLine("Called function for the {0}th time", count);
            try
            {
                return (ObjBox) func.DynamicInvoke(arguments.ToArray());
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }

    public class ObjBox
    {
        private Object obj;
        private Type type;
        public System.Reflection.MethodInfo converter;

        public ObjBox(Object _obj, Type _type)
        {
            if (!hasExternalType(_type, _obj))
            {
                this.obj = _obj;
                this.type = _type;
            }


            converter = typeof(TypeUtils).GetMethod("cast").MakeGenericMethod(type);
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(ObjBox))
            {
                ObjBox other = (ObjBox)obj;
                return (this.type == other.type) && (this.getObj().Equals(other.getObj()));
            }
            return base.Equals(obj);
        }

        //test if the _type is a .net type we do not use internally. If it is we will convert it to what it should be
        private Boolean hasExternalType(Type _type, object _obj)
        {
            if (_type == typeof(Int32))
            {
                this.type = typeof(RacketInt);
                this.obj = new RacketInt((Int32) _obj);
                return true;
            }
            if (_type == typeof(double))
            {
                this.type = typeof(RacketFloat);
                this.obj = new RacketFloat((double)obj);
                return true;
            }
            if (_type == typeof(Complex))
            {
                this.type = typeof(RacketComplex);
                this.obj = new RacketComplex((Complex)obj);
                return true;
            }

            return false;
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
