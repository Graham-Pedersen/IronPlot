using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Compiler
{
    class RacketPair
    {
        private ObjBox value;
        private ObjBox rest;

        public RacketPair(ObjBox _value, ObjBox _rest)
        {
            value = _value;
            rest = _rest;
        }

        public RacketPair()
        {
            value = null;
            rest = null;
        }

        public ObjBox car()
        {
            return value;
        }

        public ObjBox cdr()
        {
            return rest;
        }
    }
}
