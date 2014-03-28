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

            if (argTypes.Count > 0 && argTypes[0].Equals(typeof(typeListWrapper)))
            {
                return callGenConstruct(s, argTypes, objArray);
            }
            if (t == null)
            {
                throw new RuntimeException("Could not resolve type: " + s);
            }

            ConstructorInfo cons = t.GetConstructor(argTypes.ToArray());
            return new ObjBox(cons.Invoke(objArray.ToArray()), t);
        }

        private static ObjBox callGenConstruct(String typeName, List<Type> argTypes, List<Object> objArray)
        {
            
            argTypes.RemoveAt(0);
            typeListWrapper genTypes = (typeListWrapper)objArray[0];
            objArray.RemoveAt(0);

            if (typeName[0] == '\'')
            {
                typeName = typeName.Substring(1);
            }

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

        public static ObjBox callMethod(Object instance, MethodInfo m, List<Type> argTypes, List<Object> objArray)
        {
            if (m.ReturnType == typeof(void))
            {
                m.Invoke(instance, objArray.ToArray());
                return new ObjBox(new voidObj(), typeof(voidObj));
            }
            else
            {
                return new ObjBox(m.Invoke(instance, objArray.ToArray()), m.ReturnType);
            }
        }

        public static ObjBox callProperty(Object instance, PropertyInfo p, List<Type> argTypes, List<Object> objArray)
        {
            // we are setting this property
            if (objArray.Count > 0 && objArray[0].ToString() == "set")
            {
                objArray.RemoveAt(0);
                Object value = objArray[0];
                objArray.RemoveAt(0);

                if (objArray.Count == 1)
                {
                    p.SetValue(instance, value);
                }
                else
                {
                    p.SetValue(instance, value, objArray.ToArray());
                }
                return new ObjBox(new voidObj(), typeof(voidObj));
            }
            else
            {
                if (objArray.Count == 1)
                {
                    return new ObjBox(p.GetValue(instance, objArray.ToArray()), p.PropertyType);
                }

                return new ObjBox(p.GetValue(instance), p.PropertyType);
            }
        }


        public static ObjBox callField(Object instance, FieldInfo f, List<Type> argTypes, List<Object> objArray)
        {
            if (objArray.Count > 0 && objArray[0].ToString() == "set")
            {
                objArray.RemoveAt(0);
                Object value = objArray[0];
                f.SetValue(instance, value);
                return new ObjBox(new voidObj(), typeof(voidObj));
            }
            else
            {
                return new ObjBox(f.GetValue(instance), f.FieldType);
            }
        }

        public static ObjBox call(ObjBox wrapper, String s, ObjBox[] args)
        {
            Object instance = null;
            Type t;

            //if we have a static call the objBox will have a string that is the fully qualified type
            if (wrapper.getType() == (typeof(voidObj)))
            {
                String typename = (String)wrapper.getObj();
                t = Type.GetType(typename);
            }
            else
            {
                t = wrapper.getType();
                instance = wrapper.getObj();
            }

            if (t == null)
            {
                throw new RuntimeException("Could not resolve type: " + s);
            }

            // lets get all our object boxes into nice arrays
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
                return callMethod(instance, m, argTypes, objArray);
            }

            // they may be trying to access a field 
            if (f != null)
            {
                return callField(instance, f, argTypes, objArray);
            }
                
            // lets check if we are trying to set a property
            if (p != null)
            {
                return callProperty(instance, p, argTypes, objArray);
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
