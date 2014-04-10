using CompilerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Compiler
{
    public partial class DLR_Compiler
    {
        static Expression addExpr()
        {
            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Variable(typeof(ObjBox)));
            paramList.Add(Expression.Variable(typeof(ObjBox)));

            List<Expression> body = new List<Expression>();

            //unbox types
            Expression lhs = unboxValue(paramList[0], typeof(RacketNum));
            Expression rhs = unboxValue(paramList[1], typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Plus"), new Expression[] { rhs });

            body.Add(result);

            Expression lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { }, body), paramList);
            return boxFunc(lambda, paramList.Count);
        }

        static Expression subExpr()
        {
            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Variable(typeof(ObjBox)));
            paramList.Add(Expression.Variable(typeof(ObjBox)));

            List<Expression> body = new List<Expression>();

            //unbox types
            Expression lhs = unboxValue(paramList[0], typeof(RacketNum));
            Expression rhs = unboxValue(paramList[1], typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Sub"), new Expression[] { rhs });

            body.Add(result);

            Expression lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { }, body), paramList);
            return boxFunc(lambda, paramList.Count);
        }

        static Expression multExpr()
        {
            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Variable(typeof(ObjBox)));
            paramList.Add(Expression.Variable(typeof(ObjBox)));

            List<Expression> body = new List<Expression>();

            //unbox types
            Expression lhs = unboxValue(paramList[0], typeof(RacketNum));
            Expression rhs = unboxValue(paramList[1], typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Mult"), new Expression[] { rhs });

            body.Add(result);

            Expression lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { }, body), paramList);
            return boxFunc(lambda, paramList.Count);
        }

        static Expression divExpr()
        {
            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Variable(typeof(ObjBox)));
            paramList.Add(Expression.Variable(typeof(ObjBox)));

            List<Expression> body = new List<Expression>();

            //unbox types
            Expression lhs = unboxValue(paramList[0], typeof(RacketNum));
            Expression rhs = unboxValue(paramList[1], typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Div"), new Expression[] { rhs });

            body.Add(result);

            Expression lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { }, body), paramList);
            return boxFunc(lambda, paramList.Count);
        }

        static Expression modExpr()
        {
            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();
            paramList.Add(Expression.Variable(typeof(ObjBox)));
            paramList.Add(Expression.Variable(typeof(ObjBox)));

            List<Expression> body = new List<Expression>();

            //unbox types
            Expression lhs = unboxValue(paramList[0], typeof(RacketNum));
            Expression rhs = unboxValue(paramList[1], typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Mod"), new Expression[] { rhs });

            body.Add(result);

            Expression lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { }, body), paramList);
            return boxFunc(lambda, paramList.Count);
        }





        static Expression boxFunc(Expression lambda, int paramCount)
        {
            Expression func = Expression.New(
                    typeof(FunctionHolder).GetConstructor(new Type[] { typeof(Delegate), typeof(int) }),
                    lambda, Expression.Constant(paramCount, typeof(int)));

            //Now lets box this into a objBox
            return wrapInObjBox(func, Expression.Call(null, typeof(TypeUtils).GetMethod("funcType")));
        }
    }   
}
