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
                    case "+": // + can have zero args, and returns 0 in that case, so this is correct
                        {
                            int sum = 0;
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                sum += parameters[i].eval();
                            }
                            return sum;
                        }
                    case "-": // must have at least 1 arg
                        {
                            if (parameters.Count < 1)
                                throw new EvaluatorException("expected at least 1 argument, given 0");
                            if (parameters.Count == 1)
                                return -1 * parameters[0].eval();
                            int sum = parameters[0].eval() - parameters[1].eval();
                            for (int i = 2; i < parameters.Count; i++)
                            {
                                sum -= parameters[i].eval();
                            }
                            return sum;
                        }
                    case "*": // just like +
                        {
                            int sum = 1;
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                sum *= parameters[i].eval();
                            }
                            return sum;
                        }
                    case "<": // must have at least 2
                        {
                            if (parameters.Count < 2)
                                throw new EvaluatorException("expected at least 2 arguments, given 0");
                            int min = parameters[0].eval();
                            for (int i = 1; i < parameters.Count; i++)
                            {
                                if (parameters[i].eval() <= min)
                                    return false;
                            }
                            return true;
                        }
                    case ">": // same as above
                        {
                            if (parameters.Count < 2)
                                throw new EvaluatorException("expected at least 2 arguments, given 0");
                            int max = parameters[0].eval();
                            for (int i = 1; i < parameters.Count; i++)
                            {
                                if (parameters[i].eval() >= max)
                                    return false;
                            }
                            return true;
                        }
                    case "<=": // same as above, ish
                        {
                            if (parameters.Count < 2)
                                throw new EvaluatorException("expected at least 2 arguments, given 0");
                            int min = parameters[0].eval();
                            for (int i = 1; i < parameters.Count; i++)
                            {
                                if (parameters[i].eval() < min)
                                    return false;
                            }
                            return true;
                        }
                    case ">=": // same as above, ish
                        {
                            if (parameters.Count < 2)
                                throw new EvaluatorException("expected at least 2 arguments, given 0");
                            int max = parameters[0].eval();
                            for (int i = 1; i < parameters.Count; i++)
                            {
                                if (parameters[i].eval() > max)
                                    return false;
                            }
                            return true;
                        }
                    default:
                        throw new EvaluatorException("not implemented yet");
                }
            }
            else
            {
                throw new EvaluatorException("not implemented yet");
            }
        }
    }

    interface Expr
    {
        dynamic eval();

        string ToString();

    }

}
