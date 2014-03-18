using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerLib
{
    public class ParsingException : Exception
    {
        public ParsingException()
        {
        }

        public ParsingException(String message)
            : base(message)
        { }

        public ParsingException(String message, Exception inner)
            : base(message, inner)
        { }
    }
}
