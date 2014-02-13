using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Compiler
{
    static class TypeUtils
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

    class voidObj
    {
        public voidObj() { }
        public override string ToString()
        {
            return "";
        }
    }
}
