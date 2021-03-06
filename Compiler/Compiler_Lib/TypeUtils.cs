﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using CompilerLib;

namespace CompilerLib
{
    public static class TypeUtils
    {
        public static Type boolType()
        {
            return typeof(Boolean);
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

        public static Type numType()
        {
            return typeof(RacketNum);
        }

        public static Type intType()
        {
            return typeof(RacketInt);
        }

        public static Type complexType()
        {
            return typeof(RacketComplex);
        }

        public static Type floatType()
        {
            return typeof(RacketFloat);
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
            Type t = typeResolver.resolve(s);
            typelist.Add(t);
        }

        public Type resolveType()
        {
            if (typelist.Count != 1)
            {
                throw new RuntimeException("Trying to get type of list of types");
            }
            return typelist[0];
        }
    }

    public static class NetIneractLib
    {
        public static ObjBox callConstruct(String s, ObjBox[] args)
        {
            List<Type> argTypes = new List<Type>();
            List<Object> objArray = new List<Object>();
            unpackObjList(args, argTypes, objArray);

            if (argTypes.Count > 0 && argTypes[0].Equals(typeof(typeListWrapper)))
            {
                return callGenConstruct(s, argTypes, objArray);
            }
            Type t = typeResolver.resolve(s);
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
            Type t = typeResolver.resolve(typeName);

            t = t.MakeGenericType(genTypes.typelist.ToArray());

            ConstructorInfo cons = t.GetConstructor(argTypes.ToArray());
            return new ObjBox(cons.Invoke(objArray.ToArray()), t);
        }

        public static ObjBox callMethod(Object instance, MethodInfo m, List<Type> argTypes, List<Object> objArray)
        {
            /*Console.WriteLine(instance);
            Console.WriteLine(m);
            foreach (object o in objArray)
                Console.WriteLine(o);
             * */
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

        private static void unpackObjList(ObjBox[] args, List<Type> outArgTypes, List<Object> outObjArray)
        {
            foreach (ObjBox o in args)
            {
                if (o.getType().GetInterfaces().Contains(typeof(RacketNum)))
                {
                    ConvertRacketNum(o, outArgTypes, outObjArray);
                }
                else
                {
                    outArgTypes.Add(o.getType());
                    outObjArray.Add(o.getObj());
                }
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
                t = typeResolver.resolve(typename);
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
            unpackObjList(args, argTypes, objArray);

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

            String exceptionMessage; 
            if (argTypes.Count == 0)
            {
                exceptionMessage = "Type:" + t.ToString() + "does not contain matching method or property: " + s;
            }
            else
            {
                exceptionMessage =  "Type:" + t.ToString() + "does not contain method matching signature: " + s + " with types: ";
                foreach (Type sig in argTypes)
                {
                    exceptionMessage += " " + sig.ToString() + " ";
                } 

            }
            throw new RuntimeException(exceptionMessage);
        }

        private static void ConvertRacketNum(ObjBox o, List<Type> argTypes, List<object> objArray)
        {
            Type t = o.getType();
            if (t == typeof(RacketInt))
            {
                argTypes.Add(typeof(int));
                objArray.Add(((RacketInt) o.getObj()).value);
            }
            else if (t == typeof(RacketFloat))
            {
                argTypes.Add(typeof(double));
                objArray.Add(((RacketFloat)o.getObj()).value);
            }
            else if (t == typeof(RacketComplex))
            {
                argTypes.Add(typeof(Complex));
                objArray.Add(((RacketComplex)o.getObj()).value);
            }
        }
    }

    public static class typeResolver
    {
        static List<Assembly> assemblyList;
        static List<String> nameSpaceList;
        static String DLL_ASSEMBLY_PATH;

        static typeResolver()
        {
            assemblyList = new List<Assembly>();
            nameSpaceList = new List<String>();
            DLL_ASSEMBLY_PATH = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            assemblyList.Add(Assembly.Load(DLL_ASSEMBLY_PATH));
        }

        public static void import(String filename, String nSpace)
        {
            if (File.Exists(filename))
            {
                Assembly dll = Assembly.LoadFile(filename);
                if (dll == null)
                {
                    throw new RuntimeException(String.Format("Could not resolve namespace: {0} in file: {1}", nSpace, filename));
                }
                assemblyList.Add(dll);
                import(nSpace);
            }
            else
            {
                throw new RuntimeException("Could not find file: " + filename);
            }
        }

        public static void import(String nSpace)
        {
            //if the string s is a filename we should have a .dll to import
            List<Type> typelist = namespaceLookup(nSpace);
            if (typelist.Count == 0)
            {
                throw new RuntimeException("Could not resolve any types in namespace:" + nSpace);
            }
            nameSpaceList.Add(nSpace);
        }


        public static Type resolve(String typename)
        {
            Type t = null;
            //try to resolve the typename naked in case it is an absolute path
            t = Type.GetType(typename);
            if(t != null)
            {
                return t;
            }

            //resolve the typename as a type in our namespaces
            List<Type> allTypes = getAllValidTypes();
            List<Type> matchingTypes = new List<Type>();
            foreach (Type candidate in allTypes)
            {
                if (
                    candidate.Name.Equals(typename, StringComparison.Ordinal) || 
                    (candidate.Namespace + "." + candidate.Name).Equals(typename, StringComparison.Ordinal))
                {
                    matchingTypes.Add(candidate);
                }
            }

            if (matchingTypes.Count != 1)
            {
                if (matchingTypes.Count == 0)
                    throw new RuntimeException("Could not resolve type:" + typename);
                else
                    throw new RuntimeException("typename:" + typename + " matched multiple types");
            }
            else
            {
                t = matchingTypes[0];
            }
            return t;
        }

        private static List<Type> getAllValidTypes()
        {
            List<Type> ret = new List<Type>();
            foreach(String spaceName in nameSpaceList)
            {
                ret.AddRange(namespaceLookup(spaceName));
            }
            return ret;
        }

        private static List<Type> namespaceLookup(String spaceName)
        {

            List<Type> ret = new List<Type>();
            foreach(Assembly a in assemblyList)
            {
                Type[] types = a.GetTypes();
                foreach (Type t in types)
                {
                    if(String.Equals(t.Namespace, spaceName, StringComparison.Ordinal))
                    {
                        ret.Add(t);
                    }
                }
            }
            return ret;
        }

    }
   
}