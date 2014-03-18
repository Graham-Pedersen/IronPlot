using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompilerLib
{
    public static class TypeUtils
    {
        public static Type boolType()
        {
            return typeof(Boolean);
        }

        public static Type intType()
        {
            return typeof(int);
        }

        public static Type strType()
        {
            return typeof(String);
        }

        public static Type funcType()
        {
            return typeof(FunctionHolder);
        }

        public static Type listType()
        {
            throw new NotImplementedException();
        }

        public static Type iNumType()
        {
            throw new NotImplementedException();
        }

        public static Type floatType()
        {
            throw new NotImplementedException();
        }

        public static Type litearlType()
        {
            throw new NotImplementedException();
        }

        public static Type voidType()
        {
            return typeof(voidObj);
        }

        public static Type pairType()
        {
            return typeof(RacketPair);
        }

        public static T cast<T>(object o)
        {
            return (T)o;
        }
    }

    public static class NetIneractLib
    {
        public static ObjBox callConstruct(String s, ObjBox[] args)
        {
            Type t = Type.GetType(s);
            List<Type> argTypes = new List<Type>();
            List<Object> objArray = new List<Object>();
            foreach(ObjBox o in args)
            {
                argTypes.Add(o.getType());
                objArray.Add(o.getObj());
            }
            ConstructorInfo cons = t.GetConstructor(argTypes.ToArray());
            return new ObjBox(cons.Invoke(objArray.ToArray()), t);
        }

        public static ObjBox callMethod(ObjBox instance, String s, ObjBox[] args)
        {
            Type t = instance.getType();
            List<Type> argTypes = new List<Type>();
            List<Object> objArray = new List<Object>();
            foreach (ObjBox o in args)
            {
                argTypes.Add(o.getType());
                objArray.Add(o.getObj());
            }

            MethodInfo m = t.GetMethod(s, argTypes.ToArray());
            if (m.ReturnType == typeof(void))
            {
                m.Invoke(instance.getObj(), objArray.ToArray());
                return new ObjBox(new voidObj(), typeof(voidObj));
            }
            else
            {
                return new ObjBox(m.Invoke(instance.getObj(), objArray.ToArray()), m.ReturnType);
            }

        }
    }


    public class voidObj
    {
        public voidObj() { }
        public override string ToString()
        {
            return "";
        }
    }
}
