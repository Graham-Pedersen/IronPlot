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
            return typeof(RacketPair);
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

        public static Type typeListType()
        {
            return typeof(typeListWrapper);
        }

        public static T cast<T>(object o)
        {
            return (T)o;
        }
    }

    public class typeListWrapper
    {
        public List<Type> typelist;

        public typeListWrapper()
        {
            typelist = new List<Type>();
        }

        public void add(String s)
        {
            Type t = Type.GetType(s);
            if (t == null)
            {
                throw new RuntimeException("Could not resolve typename: " + s);
            }
            typelist.Add(Type.GetType(s)); 
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

            if (argTypes[0].Equals(typeof(typeListWrapper)))
            {
                return callGenConstruct(s, argTypes, objArray);
            }
            ConstructorInfo cons = t.GetConstructor(argTypes.ToArray());
            return new ObjBox(cons.Invoke(objArray.ToArray()), t);
        }

        private static ObjBox callGenConstruct(String typeName, List<Type> argTypes, List<Object> objArray)
        {
            
            argTypes.RemoveAt(0);
            typeListWrapper genTypes = (typeListWrapper)objArray[0];
            objArray.RemoveAt(0);

            typeName += "`" + genTypes.typelist.Count();
            Type t = Type.GetType(typeName);

            if (t == null)
            {
                throw new RuntimeException("Could not resolve type: " + typeName);
            }

            t = t.MakeGenericType(genTypes.typelist.ToArray());

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
            FieldInfo f = t.GetField(s);
            PropertyInfo p = t.GetProperty(s);

            // Case where we are calling a method
            if (m != null)
            {
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

            // they may be trying to access a field 
            if (f != null)
            {
                if (objArray.Count > 0 && objArray[0].ToString() == "set")
                {
                    objArray.RemoveAt(0);
                    Object value = objArray[0];
                    f.SetValue(instance.getObj(), value);
                    return new ObjBox(new voidObj(), typeof(voidObj));
                }
                else
                {
                    return new ObjBox(f.GetValue(instance.getObj()), f.FieldType);
                }
            }

            // lets check if we are trying to set a property
            if (p != null)
            {
                // we are setting this property
                if (objArray.Count > 0 && objArray[0].ToString() == "set")
                {
                    objArray.RemoveAt(0);
                    Object value = objArray[0];
                    objArray.RemoveAt(0);

                    if (objArray.Count > 0)
                    {
                        p.SetValue(instance.getObj(), value, objArray.ToArray());
                    }
                    else
                    {
                        p.SetValue(instance.getObj(), value);
                    }
                    return new ObjBox(new voidObj(), typeof(voidObj));
                }
                else
                {
                    return new ObjBox(p.GetValue(instance.getObj(), objArray.ToArray()), p.PropertyType);
                }
            }
            throw new RuntimeException("Could not resolve method or field: " + s);
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
