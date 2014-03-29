using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerLib
{
    public static class FunctionLib
    {
        public static ObjBox Map(FunctionHolder function, List<RacketPair> lists)
        {
            List<ObjBox> returnedValues = new List<ObjBox>();
            List<Object> args = new List<Object>();
            bool restNull = false;
            int listLength = -1;
            while(! restNull)
            {
                args.Clear();
                for(int i = 0; i < lists.Count; i++)
                {
                    if (listLength == -1)
                        listLength = lists[i].length();
                    if (lists[i].length() != listLength)
                        throw new RuntimeException("Lists must be of same length");

                    args.Add(lists[i].car());
                    ObjBox rest = lists[i].cdr();

                    if (rest.getType() == typeof(voidObj))
                    {
                        restNull = true;
                    }
                    else
                    {
                        lists[i] = (RacketPair)rest.getObj();
                    }
                }
                returnedValues.Add(function.invoke(args));
            }
            
            return new ObjBox(new RacketPair(returnedValues), typeof(RacketPair));
        }
    }


    public class RacketPair
    {
        private ObjBox value;
        private ObjBox rest;
        private Boolean Null;
        private int Length;

        public RacketPair(ObjBox _value, ObjBox _rest)
        {
            value = _value;
            rest = _rest;
            Null = false;
            if (_rest.getType() == typeof(voidObj))
                Length = 1;
            else
                Length = 2;
        }

        public RacketPair()
        {
            value = null;
            rest = null;
            Null = true;
            Length = 0;
        }

        public Boolean isNull()
        {
            return Null || value.getType() == typeof(voidObj);
        }

        public int length()
        {
            return Length;
        }

        public RacketPair(List<ObjBox> list)
        {
            if (list.Count == 0)
            {
                value = null;
                rest = null;
                Null = true;
                Length = 0;
                return;
            }
            value = list[0];
            Null = false;
            list.RemoveAt(0);
            Length = list.Count;
            rest = new ObjBox(new RacketPair(list), typeof(RacketPair));
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

        public override String ToString()
        {
            String lhs;
            String rhs;

            if (value.getType() == typeof(RacketPair))
            {
                lhs = ((RacketPair)value.getObj()).printList();
            }
            else { lhs = value.getObj().ToString(); }

            if (rest.getType() == typeof(RacketPair))
            {
                rhs = ((RacketPair)rest.getObj()).printList();
            }
            else 
            {
                if (rest.getType() == typeof(voidObj))
                {
                    return lhs;
                }
                rhs = rest.getObj().ToString(); 
            }

            return "(" + lhs + " " +  rhs + ")";
        }   

        public String printList()
        {
            String lhs;
            String rhs;
            if (value == null)
                return "";

            else if (value.getType() == typeof(RacketPair))
            {
                lhs = ((RacketPair)value.getObj()).printList();
            }
            else { lhs = value.getObj().ToString(); }
            if (rest == null)
                rhs = "";

            else if (rest.getType() == typeof(RacketPair))
            {
                rhs = ((RacketPair)rest.getObj()).printList();
            }
            else 
            {
                if (rest.getType() == typeof(voidObj))
                {
                    return lhs;
                } 
                rhs = rest.getObj().ToString();
            }

            return lhs + " " +  rhs;
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
