using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Compiler
{
    class RuntimeException : Exception
    {
        public RuntimeException()
        {
        }

        public  RuntimeException(String message)
            : base(message)
        { }

        public  RuntimeException(String message, Exception inner)
            : base(message, inner)
        { }
    }
}
