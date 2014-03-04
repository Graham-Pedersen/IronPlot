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
        private Boolean Null;

        public RacketPair(ObjBox _value, ObjBox _rest)
        {
            value = _value;
            rest = _rest;
            Null = false;
        }

        public RacketPair()
        {
            value = null;
            rest = null;
            Null = true;
        }

        public Boolean isNull()
        {
            return Null;
        }

        public ObjBox car()
        {
            if (Null)
            {
                throw new RuntimeException("Contract violation car excpected pair was given null");
            }
            return value;
        }

        public ObjBox cdr()
        {
            if (Null)
            {
                throw new RuntimeException("Contract violation cdr excpected pair was given null");
            }
            return rest;
        }
    }

    public class primeGener : System.Collections.IEnumerable
    {
        static bool visited = true;

        bool[] seive;
        int current;
        int max;
        bool done;

        // a generator that produces prime numbers
        public primeGener(int _max = 101)
        {
            max = _max;
            done = false;
            seive = new bool[max];
            current = 2;
        }

        public int getNext()
        {
            if (done)
            {
                return -1;
            }
            int ret = current;
            int visit = 2;
            while ( (visit*current) < max)
            {
                seive[visit*current] = visited;
                visit += 1;
            }

            current += 1;
            while (seive[current] == visited)
            {
                current += 1;
                if (current == max)
                {
                    done = true;
                    break;
                }
            }

            return ret;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            while (true)
            {
                int ret = getNext();
                if (done)
                {
                    yield return ret;
                    break;
                }
                yield return ret;
            }
        }
    }
}
