using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public static class BuiltIn
    {
        /* returns true if function is a builtin
         * function, false otherwise
         */
      //  HashSet<string> = new HashSet<string>(); // want to set up eventually
        public static bool Lookup(string function) // not going to be used
        {
            if(function == "+" || function == "-" ||
                function == "/" || function == "modulo" ||
                function == "<" || function == "<=" ||
                function == ">" || function == ">=" ||
                function == "*" || function == "cons" ||
                function == "car" || function == "cdr" ||
                function == "rest" || function == "first" ||
                function == "map" || function == "list" ||
                function == "null?")
                return true;
            else
                return false;
        }

        public static dynamic Call(string function, List<Expr> args, Dictionary<string, Expr> env)
        {
            switch (function)
            {
                case "map":
                    return Map(args, env);
                case "rest":
                    return Cdr(args, env);
                case "first":
                    return Car(args, env);
                case "list":
                    return List(args, env);
                case "cons":
                    return Cons(args, env);
                case "car":
                    return Car(args, env);
                case "cdr":
                    return Cdr(args, env);
                case "+":
                    return Plus(args, env);
                case "-":
                    return Minus(args, env);
                case "*":
                    return Mult(args, env);
                case "modulo":
                    return Modulo(args, env);
                case "/":
                    return Division(args, env);
                case "<":
                    return LessThan(args, env);
                case "<=":
                    return LessThanEqual(args, env);
                case ">":
                    return GreaterThan(args, env);
                case ">=":
                    return GreaterThanEqual(args, env);
                case "null?":
                    return NullHuh(args, env);
                default:
                    throw new EvaluatorException(String.Format("{0}: undefined", function));
            }
        }

        private static Expr List(List<Expr> args, Dictionary<string, Expr> env)
        {
            if (args.Count == 0)
                return new EmptyExpr();
            Dictionary<string, Expr> copy = new Dictionary<string, Expr>(env);
            Expr first = args[0].eval(copy);
            args.RemoveAt(0);
            return new ConsExpr(first, List(args, env));
        }

        private static Expr Map(List<Expr> args, Dictionary<string, Expr> env)
        {
            // first thing in args should be a procedure
            if(args.Count < 2)
                throw new EvaluatorException("the expected number of arguments does not match the given number");

            Expr proc = args[0];//.eval(env);
            List<Expr> evaled_lists = new List<Expr>();
            List<Expr> proc_calls = new List<Expr>();
            Expr arg;
            for(int i = 1; i < args.Count; i ++)
            {
                arg = args[i].eval(env);
                if(arg.GetType() != typeof(ConsExpr))
                    throw new EvaluatorException("map: contract violation\n expected: list?");
                evaled_lists.Add((ConsExpr)arg);
            }
            while(evaled_lists[0].GetType() != typeof(EmptyExpr))
            {
                List<Expr> proc_args = new List<Expr>();
                for(int j = 0; j < evaled_lists.Count; j ++)
                {
                    if (evaled_lists[j].GetType() == typeof(EmptyExpr))
                        throw new EvaluatorException("map: lists must be of same length");
                    ConsExpr arg_ = (ConsExpr) evaled_lists[j];
                    // check that all lists are proper lists
                    if(arg_.getRest().GetType() != typeof(ConsExpr) &&
                        arg_.getRest().GetType() != typeof(EmptyExpr))
                        throw new EvaluatorException("map: contract violation\n expected: list?");
                    proc_args.Add(arg_.getFirst());
                    evaled_lists[j] = arg_.getRest();
                }
                proc_calls.Add(new AppExpr(proc, proc_args, env));
            }
            // check that all lists end
            for(int j = 1; j < evaled_lists.Count; j ++)
            {
                if(evaled_lists[j].GetType() != typeof(EmptyExpr))
                    throw new EvaluatorException("map: all lists must have same size");
            }

            return List(proc_calls, env);
        }
        private static Expr Cdr(List<Expr> arg, Dictionary<string, Expr> env)
        {
            if (arg.Count != 1)
                throw new EvaluatorException("expected cons");
            Expr cons = arg[0].eval(env);
            if (cons.GetType() != typeof(ConsExpr))
                throw new EvaluatorException("cdr: contract violation\n expected: pair?");

            ConsExpr cons_ = (ConsExpr)cons;
            return cons_.getRest();
        }

        private static Expr Car(List<Expr> arg, Dictionary<string, Expr> env)
        {
            if (arg.Count != 1)
                throw new EvaluatorException("expected cons");
            Expr cons = arg[0].eval(env);
            if (cons.GetType() != typeof(ConsExpr))
                throw new EvaluatorException("car: contract violation\n expected: pair?");

            ConsExpr cons_ = (ConsExpr)cons;
            return cons_.getFirst();
        }

        private static ConsExpr Cons(List<Expr> args, Dictionary<string, Expr> env)
        {
            if (args.Count != 2)
                throw new EvaluatorException("the expected number of arguments does not match the given number\n expected: 2");


            return new ConsExpr(args[0].eval(env), args[1].eval(env));
        }

        private static NumExpr Plus(List<Expr> args, Dictionary<string, Expr> env)
        {
            Expr arg;
            Expr res;
            int len = args.Count;
            Int64 sum = 0;
            for (int i = 0; i < len; i++)
            {
                arg = args[i];
                res = arg.eval(env);
                if (res.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                NumExpr num_res = (NumExpr)res;
                sum = sum + num_res.getValue();
            }
            return new NumExpr(sum);
        }

        private static NumExpr Minus(List<Expr> args, Dictionary<string, Expr> env)
        {
            Expr arg;
            int len = args.Count;
            if (len < 1)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 1", "\n"));
            arg = args[0];
            Expr res = arg.eval(env);
            if (res.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            NumExpr res_num = (NumExpr)res;
            long f_num = res_num.getValue();
            if (len == 1)   
                return new NumExpr(-1 * f_num);


            long sum = f_num;
            for (int i = 1; i < len; i++)
            {
                arg = args[i];
                res = arg.eval(env);
                if(res.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                res_num = (NumExpr)res;
                sum = sum - res_num.getValue(); 
            }
            return new NumExpr(sum);
        }

        private static NumExpr Mult(List<Expr> args, Dictionary<string, Expr> env)
        {
            Expr arg;
            Expr res;
            int len = args.Count;
            Int64 sum = 1;
            NumExpr res_num;
            for (int i = 0; i < len; i++)
            {
                arg = args[i];
                res = arg.eval(env);
                if (res.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                res_num = (NumExpr)res;
                sum = sum * res_num.getValue();
            }
            return new NumExpr(sum);
        }

        private static NumExpr Modulo(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if(len != 2)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: 2", "\n"));
            Expr res1 = args[0].eval(env);
            Expr res2 = args[1].eval(env);
            if(res1.GetType() != typeof(NumExpr) || res2.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            NumExpr num_res1 = (NumExpr)res1;
            NumExpr num_res2 = (NumExpr)res2;
            return new NumExpr(num_res1.getValue() % num_res2.getValue());
        }

        private static NumExpr Division(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if(len < 1)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 1", "\n"));
            Expr f;
            f = args[0].eval(env);
            if (f.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            NumExpr num_f = (NumExpr)f;
            long num = num_f.getValue();
            if (len == 1)
            {
                if (num == 0)
                    throw new EvaluatorException("divide by zero");
                return new NumExpr(1 / num);
            }
            Expr arg;
            NumExpr n;
            long res = num;
            for (int i = 1; i < len; i++)
            {
                arg = args[i].eval(env);
                if (arg.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                n = (NumExpr)arg;
                res = res / n.getValue();
            }
            return new NumExpr(res);
        }

        private static BoolExpr LessThan(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if(len < 2)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 2", "\n"));
            Expr first = args[0].eval(env);
            NumExpr f_num;
            if (first.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            f_num = (NumExpr)first;
            long max = f_num.getValue();
            Expr arg;
            for (int i = 1; i < len; i++)
            {
                arg = args[i].eval(env);
                if (arg.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                f_num = (NumExpr)arg;
                long num = f_num.getValue();
                if (num <= max)
                    return new BoolExpr(false);
                max = num;
            }
            return new BoolExpr(true);
        }

        private static BoolExpr GreaterThan(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if (len < 2)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 2", "\n"));
            Expr first = args[0].eval(env);
            NumExpr f_num;
            if (first.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            f_num = (NumExpr)first;
            long min = f_num.getValue();
            Expr arg;
            for (int i = 1; i < len; i++)
            {
                arg = args[i].eval(env);
                if (arg.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                f_num = (NumExpr)arg;
                long num = f_num.getValue();
                if (num >= min)
                    return new BoolExpr(false);

                min = num;
            }
            return new BoolExpr(true);
        }

        private static BoolExpr LessThanEqual(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if (len < 2)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 2", "\n"));
            Expr first = args[0].eval(env);
            if (first.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            NumExpr num_f = (NumExpr)first;
            long max = num_f.getValue();
            Expr arg;
            for (int i = 1; i < len; i++)
            {
                arg = args[i].eval(env);
                if (arg.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                num_f = (NumExpr)arg;
                long num = num_f.getValue();
                if (num < max)
                    return new BoolExpr(false);

                max = num;
            }
            return new BoolExpr(true);
        }

        private static BoolExpr GreaterThanEqual(List<Expr> args, Dictionary<string, Expr> env)
        {
            int len = args.Count;
            if (len < 2)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: at least 2", "\n"));
            Expr first = args[0].eval(env);
            if (first.GetType() != typeof(NumExpr))
                throw new EvaluatorException("expected number?");
            NumExpr num_f = (NumExpr)first;
            long min = num_f.getValue();
            Expr arg;
            for (int i = 1; i < len; i++)
            {
                arg = args[i].eval(env);
                if (arg.GetType() != typeof(NumExpr))
                    throw new EvaluatorException("expected number?");
                num_f = (NumExpr)arg;
                long num = num_f.getValue();
                if (num > min)
                    return new BoolExpr(false);

                min = num;
            }
            return new BoolExpr(true);
        }

        private static BoolExpr NullHuh(List<Expr> args, Dictionary<string, Expr> env)
        {
            if(args.Count != 1)
                throw new EvaluatorException(String.Format("the expected number of arguments does not match the given number{0} expected: 1", "\n"));

            if (args[0].eval(env).GetType() == typeof(EmptyExpr))
                return new BoolExpr(true);
            return new BoolExpr(false);
        }
    }
}
