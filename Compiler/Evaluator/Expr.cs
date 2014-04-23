using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public class StrExpr : Expr
    {
        string val;
        public StrExpr(string str)
        {
            this.val = str;
        }

        public dynamic eval(Dictionary<string,Expr> env)
        {
            return this;
        }

        public override string ToString()
        {
            return val;
        }
    }

    public class AndExpr : Expr
    {
        List<Expr> args;
        public AndExpr(List<Expr> args)
        {
            this.args = args;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            Expr ret = new BoolExpr(true);
            BoolExpr boolcast;
            for (int i = 0; i < args.Count; i++)
            {
                ret = args[i].eval(env);
                if (ret.GetType() == typeof(BoolExpr))
                {
                    boolcast = (BoolExpr)ret;
                    if (!boolcast.getValue())
                        return new BoolExpr(false);
                }
            }
            return ret;
        }
    }

    public class OrExpr : Expr
    {
        List<Expr> args;
        public OrExpr(List<Expr> args)
        {
            this.args = args;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            Expr eval_res;
            BoolExpr boolcast;
            for (int i = 0; i < args.Count; i++)
            {
                eval_res = args[i].eval(env);
                if (eval_res.GetType() == typeof(BoolExpr))
                {
                    boolcast = (BoolExpr)eval_res;
                    if (!boolcast.getValue())
                        continue;
                    else
                        return new BoolExpr(true);
                }
                else
                    return eval_res;
            }
            return new BoolExpr(false); // changed this jsut now
        }
    }

    public class IfExpr : Expr
    {
        Expr cond;
        Expr then;
        Expr else_;

        public IfExpr(Expr cond, Expr then, Expr else_)
        {
            this.cond = cond;
            this.then = then;
            this.else_ = else_;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            
            if(cond.GetType() == typeof(AtomDefExpr) || cond.GetType() == typeof(FuncDefExpr)
                || then.GetType() == typeof(AtomDefExpr) || then.GetType() == typeof(FuncDefExpr)
                || else_.GetType() == typeof(AtomDefExpr) || else_.GetType() == typeof(FuncDefExpr))
                throw new EvaluatorException("define: not allowed in an expression context");

            Expr res = cond.eval(env);

            if (res.GetType() == typeof(BoolExpr))
            {
                BoolExpr bool_res = (BoolExpr)res;
                if (!bool_res.getValue()) // evaluate else_
                    return else_.eval(env);
                else
                    return then.eval(env);
            }
            else if(res.GetType() == typeof(EnvExpr))
                throw new EvaluatorException("define: not allowed in an expression context");

            return then.eval(env);
        }

        public override string ToString()
        {
            return base.ToString(); // dont know what to print
        }
    }

    public class BoolExpr : Expr
    {
        bool val;
        public BoolExpr(bool val)
        {
            this.val = val;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return this;
        }

        public override string ToString()
        {
            if (val)
                return "#t";
            else
                return "#f";
        }

        public bool getValue()
        {
            return val;
        }
    }
    public class ConsExpr : Expr
    {
        Expr first;
        Expr rest;

        public ConsExpr(Expr first, Expr rest)
        {
            this.first = first;
            this.rest = rest;
        }

        public dynamic eval(Dictionary<string, Expr> env)
        {
            return this;
        }

        public Expr getRest()
        {
            return rest;
        }
        public Expr getFirst()
        {
            return first;
        }

        public override string ToString()
        {
            if (rest.GetType() == typeof(ConsExpr))
                return String.Format("({0} {1})", first.ToString(), rest.ToString());
            else if (rest.GetType() == typeof(EmptyExpr))
                return string.Format("({0})", first.ToString());
            else
                return string.Format("({0} . {1})", first.ToString(), rest.ToString());
        }
    }

    public class EmptyExpr : Expr // represents empty list
    {
        public EmptyExpr()
        {

        }

        public dynamic eval(Dictionary<string,Expr> env)
        {
            return this;
        }

        public override string ToString()
        {
            return "()";
        }
    }

    public class EnvExpr : Expr
    {
        private Dictionary<string,Expr> env;

        public EnvExpr(Dictionary<string,Expr> env)
        {
            this.env = new Dictionary<string, Expr>(env);
        }

       public dynamic eval(Dictionary<string,Expr> env)
        {
            return this.env;
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

       public dynamic eval(Dictionary<string,Expr> env)
        {
            return new PrimClosExpr(primitive, env);
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

       public dynamic eval(Dictionary<string,Expr> env)
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
            {
                return env[name];
            }
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
            this.env = new Dictionary<string, Expr>(env);
        }

        public dynamic apply(List<Expr> parameters)
        {
            return BuiltIn.Call(fun, parameters, env);
        }

     public dynamic eval(Dictionary<string,Expr> env)
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

        public ClosExpr(List<string> args, List<Expr> body, Dictionary<string, Expr> env)
        {
            this.env = new Dictionary<string, Expr>(env);
            this.args = args;
            this.body = body;
        }

       public dynamic eval(Dictionary<string,Expr> env)
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
                if (parameters[i].GetType() == typeof(AtomDefExpr) || parameters[i].GetType() == typeof(FuncDefExpr))
                    throw new EvaluatorException("define: not allowed in an expression context");
                env.Add(args[i], parameters[i].eval(env));
            }

            int body_len = body.Count;
            dynamic ret = null;
            for(int j = 0; j < body_len; j ++)
            {
                ret = body[j].eval(env);
                if (ret.GetType() == typeof(EnvExpr))
                    env = new Dictionary<string,Expr>(ret.eval(env));
            }
            if (ret.GetType() == typeof(EnvExpr))
                throw new EvaluatorException("no expression after a sequence of internal definitions");
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

        /* if in environment, throw exception
         * add to environment
         * return new environment
         */
        public dynamic eval(Dictionary<string,Expr> env)
        {
            Dictionary<string, Expr> new_env = new Dictionary<string, Expr>(env);
            if (new_env.ContainsKey(id))
                new_env.Remove(id);

            new_env.Add(id, exp.eval(env));
            return new EnvExpr(new_env);

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
            ClosExpr value = new ClosExpr(args, body, env);
            Dictionary<string, Expr> new_env = new Dictionary<string, Expr>(env);
            if (new_env.ContainsKey(name))
                new_env.Remove(name);
            new_env.Add(name, value);
            return new EnvExpr(new_env);
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
            this.env = new Dictionary<string,Expr>(env);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public dynamic eval(Dictionary<string,Expr> env)
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
