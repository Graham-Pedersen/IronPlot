using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CompilerLib
{
    public static class FunctionLib
    {
        public static ObjBox Reverse(RacketPair list)
        {
            bool restNull = false;
            if (list.isNull())
                return new ObjBox(list, typeof(RacketPair));

            RacketPair accu = new RacketPair(list.car(), 
                new ObjBox(new RacketPair(), typeof(RacketPair)));

                list = (RacketPair)list.cdr().getObj();
            
            while (!list.isNull())
            {
                accu = new RacketPair(list.car(),
                    new ObjBox(accu, typeof(RacketPair)));

                    list = (RacketPair)list.cdr().getObj();
            }
            return new ObjBox(accu, typeof(RacketPair));
        }
        public static ObjBox Map(FunctionHolder function, List<RacketPair> lists)
        {
            List<ObjBox> returnedValues = new List<ObjBox>();
            List<Object> args = new List<Object>();
            bool restNull = lists[0].isNull();
            int listLength = -1;
            while(!restNull)
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

                    if (((RacketPair) rest.getObj()).isNull())
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

        public static ObjBox Foldl(FunctionHolder function, ObjBox init_param, List<RacketPair> lists)
        {
            ObjBox init = init_param;
            List<Object> args = new List<Object>();
            bool restNull = lists[0].isNull();
            int listLength;

            if (function.param_num != lists.Count + 1)
                throw new RuntimeException("Wrong number of arguments for given procedure");

            while (!lists[0].isNull())
            {
                args.Clear();
            //    args.Add(init);
                listLength = -1;
                for (int i = 0; i < lists.Count; i++)
                {
                    if (listLength == -1)
                        listLength = lists[i].length();
                    if (lists[i].length() != listLength)
                        throw new RuntimeException("Lists must be of same length");
                    if (lists[i].isNull())
                        throw new RuntimeException("Lists must be of same length");

                    args.Add(lists[i].car());
                    ObjBox rest = lists[i].cdr();
                    /*
                    if (rest.getType() == typeof(voidObj))
                    {
                        restNull = true;
                    }
                    else
                    { */
                        lists[i] = (RacketPair)rest.getObj();
                }
                args.Add(init);
                init = function.invoke(args); // should be okay if making copies
            //    returnedValues.Add(function.invoke(args));
            }

            return init;
        }

        public static ObjBox Apply(FunctionHolder function, RacketPair list)
        {
            int param_num = function.param_num;
            List<Object> args = new List<Object>();
            ObjBox fun_call;

            if (list.length() != param_num)
                throw new RuntimeException("The expected number of arguments does not match the given number");
            int length = list.length();

            for (int i = 0; i < length; i++)
            {
                args.Add(list.car());
                ObjBox rest = list.cdr();

                if (rest.getType() == typeof(voidObj))
                {
                    break;
                }
                else
                {
                    list = (RacketPair)rest.getObj();
                }
            }
            fun_call = function.invoke(args);

            return fun_call;
        }

    }


    public interface RacketNum
    {
        ObjBox Plus(RacketNum other);
        ObjBox Sub(RacketNum other);
        ObjBox Mult(RacketNum other);
        ObjBox Div(RacketNum other);
        ObjBox Mod(RacketNum other);

        Boolean RealQ(RacketNum other);
        Boolean ComplexQ(RacketNum other);
        Boolean FloatQ(RacketNum other);
        Boolean IntegerQ(RacketNum other);

        Boolean lessThan(RacketNum other);
        Boolean lessThanEqual(RacketNum other);
        Boolean greaterThan(RacketNum other);
        Boolean greaterThanEqual(RacketNum other);
    }

    public class RacketInt : RacketNum
    {
        public int value { private set;  get; }

        public RacketInt(int _value)
        {
            value = _value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().GetInterfaces().Contains(typeof(RacketNum)))
            {
                if(obj.GetType() == typeof(RacketInt))
                {
                    return this.value == ((RacketInt) obj).value;
                }
                if(obj.GetType() == typeof(RacketFloat))
                {
                    return this.value == ((RacketFloat) obj).value;
                }
                if(obj.GetType() == typeof(RacketComplex))
                {
                    return this.value == ((RacketComplex) obj).value;
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static implicit operator int(RacketInt m)
        {
            return m.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public ObjBox Plus(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketInt(value + ((RacketInt)other).value), typeof(RacketInt));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value + ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value + ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Sub(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketInt(value - ((RacketInt)other).value), typeof(RacketInt));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value - ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value - ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mult(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketInt(value * ((RacketInt)other).value), typeof(RacketInt));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value * ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value * ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Div(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(((Double) value) / ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value / ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox( new RacketComplex(value / ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mod(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketInt(value % ((RacketInt)other).value), typeof(RacketInt));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value % ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                throw new InvalidOperationException("Cannot use mod operation with a complex number");
            }
            throw new NotImplementedException();
        }

        bool RacketNum.RealQ(RacketNum other)
        {
            return true;
        }

        bool RacketNum.ComplexQ(RacketNum other)
        {
            return false;
        }

        bool RacketNum.FloatQ(RacketNum other)
        {
            return false;
        }

        bool RacketNum.IntegerQ(RacketNum other)
        {
            return true;
        }


        public bool lessThan(RacketNum other)
        {
            if(other.GetType() == typeof(RacketInt))
                return this.value < ((RacketInt) other).value;
            if(other.GetType() == typeof(RacketFloat))
                return this.value < ((RacketFloat) other).value;

            throw new NotImplementedException();
        }

        public bool lessThanEqual(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value <= ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value <= ((RacketFloat)other).value;

            throw new NotImplementedException();
        }

        public bool greaterThan(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value > ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value > ((RacketFloat)other).value;

            throw new NotImplementedException();
        }

        public bool greaterThanEqual(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value >= ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value >= ((RacketFloat)other).value;

            throw new NotImplementedException();
        }
    }

    public class RacketFloat : RacketNum
    {
        public Double value { private set; get; }
        
        public RacketFloat(Double _value)
        {
            value = _value;
        }

        public static implicit operator Double(RacketFloat m)
        {
            return m.value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().GetInterfaces().Contains(typeof(RacketNum)))
            {
                if (obj.GetType() == typeof(RacketInt))
                {
                    return this.value == ((RacketInt)obj).value;
                }
                if (obj.GetType() == typeof(RacketFloat))
                {
                    return this.value == ((RacketFloat)obj).value;
                }
                if (obj.GetType() == typeof(RacketComplex))
                {
                    return this.value == ((RacketComplex)obj).value;
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public ObjBox Plus(RacketNum other)
        {
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value + ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(value + ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value + ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Sub(RacketNum other)
        {
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value - ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(value - ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value - ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mult(RacketNum other)
        {
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value * ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(value * ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value * ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Div(RacketNum other)
        {
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketFloat(value / ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(value / ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value / ((RacketComplex)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mod(RacketNum other)
        {
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox( new RacketFloat(value % ((RacketFloat)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketFloat(value % ((RacketInt)other).value), typeof(RacketFloat));
            }
            if (other.GetType() == typeof(RacketComplex))
            {
                throw new InvalidOperationException("Cannot use mod operation with a complex number");
            }
            throw new NotImplementedException();
        }

        public bool RealQ(RacketNum other)
        {
            return true;
        }

        public bool ComplexQ(RacketNum other)
        {
            return false;
        }

        public bool FloatQ(RacketNum other)
        {
            return true;
        }

        public bool IntegerQ(RacketNum other)
        {
            return false;
        }

        public bool lessThan(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value < ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value < ((RacketFloat)other).value;

            throw new NotImplementedException();
        }

        public bool lessThanEqual(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value <= ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value <= ((RacketFloat)other).value;

            throw new NotImplementedException();
        }

        public bool greaterThan(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value > ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value > ((RacketFloat)other).value;

            throw new NotImplementedException();
        }

        public bool greaterThanEqual(RacketNum other)
        {
            if (other.GetType() == typeof(RacketInt))
                return this.value >= ((RacketInt)other).value;
            if (other.GetType() == typeof(RacketFloat))
                return this.value >= ((RacketFloat)other).value;

            throw new NotImplementedException();
        }
    }

    public class RacketComplex : RacketNum
    {
        public Complex value { private set; get; }

        public RacketComplex(Complex _value)
        {
            value = _value;
        }

        public static implicit operator Complex(RacketComplex m)
        {
            return m.value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().GetInterfaces().Contains(typeof(RacketNum)))
            {
                if (obj.GetType() == typeof(RacketInt))
                {
                    return this.value == ((RacketInt)obj).value;
                }
                if (obj.GetType() == typeof(RacketFloat))
                {
                    return this.value == ((RacketFloat)obj).value;
                }
                if (obj.GetType() == typeof(RacketComplex))
                {
                    return this.value == ((RacketComplex)obj).value;
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public ObjBox Plus(RacketNum other)
        {
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value + ((RacketComplex)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketComplex(value + ((RacketInt)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketComplex(value + ((RacketFloat)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Sub(RacketNum other)
        {
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value - ((RacketComplex)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketComplex(value - ((RacketInt)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketComplex(value - ((RacketFloat)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mult(RacketNum other)
        {
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value * ((RacketComplex)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketComplex(value * ((RacketInt)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketComplex(value * ((RacketFloat)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Div(RacketNum other)
        {
            if (other.GetType() == typeof(RacketComplex))
            {
                return new ObjBox(new RacketComplex(value / ((RacketComplex)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketInt))
            {
                return new ObjBox(new RacketComplex(value / ((RacketInt)other).value), typeof(RacketComplex));
            }
            if (other.GetType() == typeof(RacketFloat))
            {
                return new ObjBox(new RacketComplex(value / ((RacketFloat)other).value), typeof(RacketComplex));
            }
            throw new NotImplementedException();
        }

        public ObjBox Mod(RacketNum other)
        {
            throw new InvalidOperationException("Cannot use mod operation with a complex number");
        }

        public bool RealQ(RacketNum other)
        {
            return false;
        }

        public bool ComplexQ(RacketNum other)
        {
            return true;
        }

        public bool FloatQ(RacketNum other)
        {
            return false;
        }

        public bool IntegerQ(RacketNum other)
        {
            return false;
        }


        public bool lessThan(RacketNum other)
        {
            throw new NotImplementedException();
        }

        public bool lessThanEqual(RacketNum other)
        {
            throw new NotImplementedException();
        }

        public bool greaterThan(RacketNum other)
        {
            throw new NotImplementedException();
        }

        public bool greaterThanEqual(RacketNum other)
        {
            throw new NotImplementedException();
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

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(RacketPair))
            {
                RacketPair other = (RacketPair) obj;
                if (this.isNull() && other.isNull())
                    return true;
                //one of us is null the other is not
                if (this.isNull() || other.isNull())
                    return false;
                return this.value.Equals(other.car()) && this.rest.Equals(other.cdr());
            }
            
            return base.Equals(obj);
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
            if (isNull())
                return "()";

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
