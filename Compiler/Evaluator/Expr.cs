using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    // represent things like +, -, etc
    public class PrimFuncExpr : Expr
    {
        private string primitive;

        public PrimFuncExpr(string prim)
        {
            this.primitive = prim;
        }

        public dynamic eval()
        {
            return primitive;
        }

        public string ToString()
        {
            return "#<procedure>";
        }
    }

    public class NumExpr : Expr
    {
        dynamic value;

        public NumExpr(dynamic val)
        {
            this.value = val;
        }

        public dynamic eval()
        {
            return value;
        }

        public string ToString()
        {
            // value should be int or something;
            return value.ToString();
        }
    }

    public class VarExpr : Expr
    {
        string name;
        string value;

        public VarExpr(string sym)
        {
            this.name = sym;
        }

        public dynamic eval()
        {
            // look up in environment?
            return null;
        }

        public string ToString()
        {
            return "";
        }
    }

    public class ClosExpr : Expr
    {
        Expr env;
        Expr func;

        public ClosExpr(Expr function, Expr env)
        {
            this.env = env;
            this.func = function;
        }

        public dynamic eval()
        {
            return null;
        }

        public string ToString()
        {
            return "#<procedure>";
        }
    }

    public class LamExpr : Expr
    {
        public LamExpr()
        {

        }

        public dynamic eval()
        {
            return null;
        }

        public string ToString()
        {
            return "#<procedure>";
        }
    }

    public class AppExpr : Expr
    {
        Expr app;
        List<Expr> parameters;

        public AppExpr(Expr app, List<Expr> parameters)
        {
            this.app = app;
            this.parameters = parameters;
        }

        public dynamic eval()
        {
            dynamic app_res = app.eval();
            if (app.GetType() == typeof(PrimFuncExpr))
            {
                switch ((string)app_res)
                {
                    case "+":
                        return null;
                    case "-": 
                        return null;
                    case "*": 
                        return null;
                    case "<": 
                        return null;
                    case ">": 
                        return null;
                    case "<=": 
                        return null;
                    case ">=": 
                        return null;
                }
            }
            return null;
        }

        private bool isPrimitiveFunc()
        {
            return app.GetType() == typeof(PrimFuncExpr);
        }
    }

    interface Expr
    {
        dynamic eval();

        string ToString();

    }

}
