using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public class EnvExpr : Expr
    {
        private Dictionary<string, Expr> env;

        public EnvExpr(Dictionary<string, Expr> env)
        {
            this.env = env;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return env;
        }

        override public string ToString()
        {
            return "";
        }
    }
    // represent things like +, -, etc
    public class PrimFuncExpr : Expr
    {
        private string primitive;

        public PrimFuncExpr(string prim)
        {
            this.primitive = prim;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return new PrimClosExpr(primitive, env);
            /*
            if (BuiltIn.Lookup(primitive))
                return new PrimClosExpr(primitive, env);
            else
                throw new EvaluatorException(String.Format("{0}: undefined", primitive)); */
        }

        override public string ToString()
        {
            return String.Format("#<procedure:{0}>", primitive);
        }

    }

    public class NumExpr : Expr
    {
        long value;

        public NumExpr(long val)
        {
            this.value = val;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return this;
        }

        override public string ToString()
        {
            // value should be int or something;
            return value.ToString();
        }

        public long getValue()
        {
            return value;
        }
    }

    public class VarExpr : Expr
    {
        string name;

        public VarExpr(string sym)
        {
            this.name = sym;
        }

        public dynamic eval(Dictionary<string,Expr> env)
        {
            // look up in environment?
            // if not in environment throw exception
            if (env.ContainsKey(name))
                return env[name];//.eval(env);
            else if (BuiltIn.Lookup(name))
                return new PrimFuncExpr(name).eval(env);
            else
            {
                throw new EvaluatorException(String.Format("{0}: undefined", name));
            }
        }

        override public string ToString()
        {
            return "";
        }
    }

    public class PrimClosExpr : Closure, Expr
    {
        string fun;
        Dictionary<string, Expr> env;
        public PrimClosExpr(string fun, Dictionary<string, Expr> env)
        {
            this.fun = fun;
            this.env = env;
        }

        public dynamic apply(List<Expr> parameters)
        {
            /*
            if(!BuiltIn.Lookup(fun)) // repetitive
                throw new EvaluatorException(String.Format("{0}: undefined", fun));
            */
            return BuiltIn.Call(fun, parameters, env);
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return new PrimFuncExpr(fun);
        }

        override public string ToString()
        {
            return String.Format("#<procedure:{0}>", fun);
        }
    }

    public class ClosExpr : Closure ,Expr
    {
        Dictionary<string, Expr> env;
        List<string> args;
        List<Expr> body;

        public ClosExpr(List<string> args, List<Expr> body, Dictionary<string,Expr> env)
        {
            this.env = env;
            this.args = args;
            this.body = body;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return new LamExpr(args, body);
        }

        public dynamic apply(List<Expr> parameters)
        {
            if (parameters.Count != args.Count)
                throw new EvaluatorException("the expected number of arguments does not match the given number");

            // need to bind the args to params in the environment
            int len = parameters.Count;
            for (int i = 0; i < len; i++)
            {
                env.Add(args[i], parameters[i]);
            }

            int body_len = body.Count;
            dynamic ret = null;
            for(int j = 0; j < body_len; j ++)
            {
                ret = body[j].eval(env);
                if (ret.GetType() == typeof(EnvExpr))
                    env = ret.eval(env);

            }
            return ret;
        }

        override public string ToString()
        {
            return "#<procedure>";
        }
    }

    public class LamExpr : Expr
    {
        List<string> args;
        List<Expr> body;

        public LamExpr(List<string> args, List<Expr> body)
        {
            this.args = args;
            this.body = body;
        }

        public dynamic eval(Dictionary<string,Expr> env)
        {
            return new ClosExpr(args, body, env);
        }

        override public string ToString()
        {
            return "#<procedure>";
        }
    }

    public class AtomDefExpr : Expr
    {
        string id; // should be a var, which is just a string
        Expr exp;

        public AtomDefExpr(string id, Expr exp)
        {
            this.id = id;
            this.exp = exp;
        }

        /* if in environment, remove previous definition
         * add to environment
         * return new environment
         */
        public dynamic eval(Dictionary<string, Expr> env)
        {
            if (env.ContainsKey(id))
                env.Remove(id);
            env.Add(id, exp);
            return new EnvExpr(env);
        }

        override public string ToString()
        {
            return ""; // what it should actually return
        }
    }

    public class FuncDefExpr : Expr
    {
        string name;
        List<string> args;
        List<Expr> body;

        public FuncDefExpr(string name, List<string> args, List<Expr> body)
        {
            this.name = name;
            this.args = args;
            this.body = body;
        }

        public dynamic eval(Dictionary<string,Expr> env)
        {
            if (env.ContainsKey(name)) // should i allow 
                env.Remove(name);
            ClosExpr value = new ClosExpr(args, body, env);
            env.Add(name, value);
            return new EnvExpr(env);
        }

        override public string ToString()
        {

            return ""; // what it should actually return
        }
    }

    public class AppExpr : Expr
    {
        Expr app;
        List<Expr> parameters;
        Dictionary<string,Expr> env;

        public AppExpr(Expr app, List<Expr> parameters, Dictionary<string,Expr> env)
        {
            this.app = app;
            this.parameters = parameters;
            this.env = env;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            Expr res = app.eval(env);
            Type res_type = res.GetType();
            if(!(res_type == typeof(PrimClosExpr) || res_type == typeof(ClosExpr)))
                throw new EvaluatorException("application: not a procedure");

            Closure clos = (Closure)res;
            return clos.apply(parameters);
        }
    }

    public interface Expr
    {
        dynamic eval(Dictionary<string, Expr> env);

        string ToString();

    }

    public interface Closure
    {
        dynamic apply(List<Expr> args);

    }

}
