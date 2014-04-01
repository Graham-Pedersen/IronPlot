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
            return null;
        }

        public string getPrimitive()
        {
            return primitive;
        }
    }

    public class numExpr : Expr
    {
        dynamic value;

        public numExpr(dynamic val)
        {
            this.value = val;
        }

        public dynamic eval()
        {
            return value;
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
            if (isPrimitiveFunc())
            {
                PrimFuncExpr app_ = (PrimFuncExpr)app;
                string prim = app_.getPrimitive();
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
