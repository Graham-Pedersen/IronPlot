using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SimpleSchemeParser;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using CompilerLib;

namespace DLR_Compiler
{
   public class DLR_Compiler
    {
        static ParameterExpression voidSingleton;

        public static void Main(string[] args)
        {
            //string filename = @"C:\Users\graha_000\Programing\IronPlot\test\12.plot";
            string filename = args[0];
            //Console.WriteLine("Compiling file " + filename);

            string mode = args[1];

            //set up a single instance of type void
            voidSingleton = Expression.Variable(typeof(ObjBox), "void");
            Expression voidType = Expression.Call(null, typeof(TypeUtils).GetMethod("voidType"));

            if (mode == "repl")
            {
                repl();
            }
            else
            {
                // make a new simple scheme parser
                SchemeParser ssp = new SchemeParser(filename);
                ListNode topLevelForms = ssp.parseFile();

                // these expressions will initalize the top level environment
                var makeEnv = Expression.New(typeof(CompilerLib.Environment));
                var env = Expression.Variable(typeof(CompilerLib.Environment), "env");
                var assign = Expression.Assign(env, makeEnv);

                Expression initVoidObjBox = Expression.New(
                    typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                    new Expression[] { Expression.Convert(Expression.New(typeof(voidObj).GetConstructor(new Type[] { })), typeof(Object)), voidType });

                Expression assignVoid = Expression.Assign(voidSingleton, initVoidObjBox);

                //Add the environment to the start of the program
                List<Expression> program = new List<Expression>();
                program.Add(env);
                program.Add(assign);
                program.Add(assignVoid);

                Expression ret = unboxValue(matchTopLevel(topLevelForms, env), typeof(Object));



                //Match and add the rest of the program
                program.Add(
                    Expression.Call(
                    null,
                    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }),
                        Expression.Call(ret, typeof(Object).GetMethod("ToString"))));

                //if the program is being compiled put a readkey in
                if (mode == "compile")
                {
                    program.Add(Expression.Call(typeof(Console).GetMethod("ReadKey", new Type[] { })));
                }

                //Wrap the program into a block expression
                Expression code = Expression.Block(new ParameterExpression[] { env, voidSingleton }, program);

                if (mode == "compile")
                {
                    var asmName = new AssemblyName("Foo");
                    var asmBuilder = AssemblyBuilder.DefineDynamicAssembly
                        (asmName, AssemblyBuilderAccess.RunAndSave);
                    var moduleBuilder = asmBuilder.DefineDynamicModule("Foo", "Foo.exe");
                    var typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Public);
                    var methodBuilder = typeBuilder.DefineMethod("Main",
                        MethodAttributes.Static, typeof(void), new[] { typeof(string) });

                    Expression.Lambda<Action>(code).CompileToMethod(methodBuilder);

                    typeBuilder.CreateType();
                    asmBuilder.SetEntryPoint(methodBuilder);
                    asmBuilder.Save("Foo.exe");
                }
                else if (mode == "run")
                {
                    Expression.Lambda<Action>(code).Compile()();
                    Console.ReadKey();
                }
            }
        }

        static void repl()
        {
            string input = "";
            while (true)
            {
                input += Console.ReadLine();
                Console.WriteLine(input);
            }
        }
       
        /** match all top level forms in the form of:
         * 
         *  topLevelForms ::= (<def> | <exp>)
         *
         **/
        static Expression matchTopLevel(Node tree, Expression env)
        {
           
            if (tree.isList())
            {
                ListNode list = (ListNode)tree;
                if (list.getNestingLevel() == 0)
                {
                    List<Expression> dlrTree = new List<Expression>();
                    foreach (Node n in ((ListNode)tree).getList())
                    {
                        dlrTree.Add(matchTopLevel(n, env));
                    }
                    return Expression.Block(dlrTree);
                }
                //define expression
                if (list.values[0].isLeaf() && list.values[0].getValue() == "define" )
                {
                    return defineExpr(list, env);
                }
                else if (list.values[0].isLeaf() && list.values[0].getValue() == "netuse")
                {
                    return netImportStmt(list, env);
                }
                /*
                 * bool parsed = false;
                var ret = matchExpression(list, env, out parsed);
                if (parsed)
                    return ret;
                 */
            }
            return matchExpression(tree, env);
        }


        // This matches a list and converts it into an expression
        static Expression matchExpression(Node tree, Expression env)
        {
            if (tree.isList())
            {
                ListNode list = (ListNode)tree;

                if (list.isLiteral)
                {
                    return matchLiteralList(list, env);
                }

                else if (list.values[0].isList())
                {
                    return autoInvokeLambda(list, env);
                }
                else
                {
                    //perform a function lookup first because in scheme you can overwrite language keywords
                    switch (list.values[0].getValue())
                    {

                        case "call":
                            return callNetExpr(list, env);

                        case "scall":
                            return scallNetExpr(list, env);

                        case "new":
                            return newNetObj(list, env);

                        case "typelist":
                            return newTypeList(list, env);

                        case "+":
                            return addExpr(list, env);

                        case "-":
                            return subExpr(list, env);

                        case "*":
                            return multExpr(list, env);

                        case "/":
                            return divExpr(list, env);

                        case "%":
                            return modExpr(list, env);

                        case "void":
                            return voidSingleton;

                        case "equal?":
                            return equalExpr(list, env);

                        case "not":
                            return notExpr(list, env);

                        case "lambda":
                            return lambdaExpr(list, env);

                        case "if":
                            return ifExpr(list, env);

                        case "while":
                            return whileExpr(list, env);

                        //TODO extend environment and fix this
                        case "set!":
                            return setBangExpr(list, env);

                        case "cons":
                            return consExpr(list, env);

                        case "car":
                            return carExpr(list, env);

                        case "cdr":
                            return cdrExpr(list, env);

                        case "begin":
                            return beginExpr(list, env);

                        case "displayln":
                            return displayExpr(list, env);

                        case "null?":
                            return nullCheckExpr(list, env);

                        //TODO add environment check and move above standard cases
                        default:
                            return invokeLambda(list, env);
                    }
                }
            }
            else
            {
                return matchLeaf(tree, env);
            }
        }



        private static Expression newTypeList(ListNode list, Expression env)
        {
            List<Expression> body = new List<Expression>();

            Expression cons = Expression.New(typeof(typeListWrapper).GetConstructor(new Type[] { }));
            ParameterExpression var = Expression.Parameter(typeof(typeListWrapper));
            Expression assign = Expression.Assign(var, cons);

            body.Add(assign);

            for (int i = 1; i < list.values.Count; i++)
            {
                body.Add(
                    Expression.Call(var, typeof(typeListWrapper).GetMethod("add"), 
                    unboxValue(aliasOrLiteralName(list.values[i], env), typeof(String))));
            }

            body.Add(var);
            Expression block = Expression.Block(new ParameterExpression[] { var }, body);

            return wrapInObjBox(block, Expression.Call(null, typeof(TypeUtils).GetMethod("typeListType")));
            
        }

        private static Expression notExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 2)
                throw new ParsingException("wrong number of arguments supplied for not expression");

            Expression not = Expression.Not(unboxValue(matchExpression(list.values[1], env), typeof(Boolean)));
            return wrapInObjBox(
                not,
                Expression.Call(null, typeof(TypeUtils).GetMethod("boolType")));
        }

        private static Expression netImportStmt(ListNode list, Expression env)
        {
            throw new NotImplementedException();
        }

        private static Expression scallNetExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();
            if (list.values.Count < 2)
            {
                throw new ParsingException("Failed to parse .net call expression");
            }

            ParameterExpression arr = Expression.Parameter(typeof(List<ObjBox>));
            Expression argArray = Expression.New(typeof(List<ObjBox>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(arr, argArray);
            block.Add(assign);

            
            Expression arg;
            if (list.values.Count > 2)
            {
                if (list.values[2].isLeaf() && list.values[2].getValue() == "set")
                {
                    arg = wrapInObjBox(Expression.Constant("set"), Expression.Call(null, typeof(TypeUtils).GetMethod("strType")));
                }
                else
                {
                    arg = matchExpression(list.values[2], env);
                }
                block.Add(
                 Expression.Call(
                     arr,
                     typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                     arg));
            }


            for (int i = 3; i < list.values.Count; i++)
            {
                arg = matchExpression(list.values[i], env);
                block.Add(
                    Expression.Call(
                        arr,
                        typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                         matchExpression(list.values[i], env)));
            }
            Expression obj = aliasOrLiteralName(list.values[1], env);
            Expression callStr = unboxValue(obj, typeof(String));

            block.Add(Expression.Call(null, typeof(NetIneractLib).GetMethod("call"),
                wrapInObjBox(voidSingleton, Expression.Call(null, typeof(TypeUtils).GetMethod("voidType"))),
                callStr,
                Expression.Call(arr, typeof(List<ObjBox>).GetMethod("ToArray"))));

            return Expression.Block(new ParameterExpression[] { arr }, block);
        }

        private static Expression callNetExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();
            if (list.values.Count < 3)
            {
                throw new ParsingException("Failed to parse .net call expression");
            }

            ParameterExpression arr = Expression.Parameter(typeof(List<ObjBox>));
            Expression argArray = Expression.New(typeof(List<ObjBox>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(arr, argArray);
            block.Add(assign);

            if (list.values.Count > 3)
            {
                Expression arg;
                if (list.values[3].isLeaf() && list.values[3].getValue() == "set")
                {
                    arg = wrapInObjBox(Expression.Constant("set"), Expression.Call(null, typeof(TypeUtils).GetMethod("strType")));
                }
                else
                {
                    arg = matchExpression(list.values[3], env);
                } 
                block.Add(
                 Expression.Call(
                     arr,
                     typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                     arg));
            }

            for (int i = 4; i < list.values.Count; i++)
            {
                Expression arg = matchExpression(list.values[i], env);
                block.Add(
                    Expression.Call(
                        arr,
                        typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                         arg));
            }
            Expression instance = matchExpression(list.values[1], env);
            Expression obj = aliasOrLiteralName(list.values[2], env);
            Expression callStr = unboxValue(obj, typeof(String));
            block.Add(Expression.Call(null, typeof(NetIneractLib).GetMethod("call"), 
                Expression.Convert(instance, typeof(ObjBox)), 
                callStr,
                Expression.Call(arr, typeof(List<ObjBox>).GetMethod("ToArray"))));
            return Expression.Block(new ParameterExpression[] { arr }, block);
        }

        // this checks if there is a variable in the environment
        private static Expression aliasOrLiteralName(Node tree, Expression env)
        {

            if (tree.isList())
            {
                return matchExpression(tree, env);
            }
            
            List<Expression> body = new List<Expression>();

            ParameterExpression ret = Expression.Parameter(typeof(ObjBox));
            ParameterExpression name = Expression.Parameter(typeof(String));

            Expression isInEnv;
            Expression f;
            Expression t;
            Expression isItStr;
            Expression envQuery;

            //lets get the name into expression form
            Expression rhs = Expression.Constant(tree.getValue(), typeof(String));
            Expression assign = Expression.Assign(name, rhs);
            body.Add(assign);

            envQuery = lookup(name, env);

            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("strType"));
            f = Expression.Assign(ret, wrapInObjBox(name, type));
            t = Expression.Assign(ret, envQuery);
            isInEnv = checkEnv(name, env);
            isItStr = Expression.Call(
                        Expression.Call(envQuery, typeof(ObjBox).GetMethod("getType")),
                        typeof(Object).GetMethod("Equals", new Type[] { typeof(Object) }),
                        Expression.Constant(typeof(String), typeof(Type)));

            Expression firstIf = Expression.IfThenElse(
                isInEnv,
                Expression.IfThenElse(isItStr, t, f),
                f);

            body.Add(firstIf);
            body.Add(ret);

            return Expression.Block(new ParameterExpression[] { name, ret }, body);
        }

        private static Expression newNetObj(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();
            if (list.values.Count < 2)
            {
                throw new ParsingException("Failed to parse .net new expression");
            }

            ParameterExpression arr = Expression.Parameter(typeof(List<ObjBox>));
            Expression argArray = Expression.New(typeof(List<ObjBox>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(arr, argArray);
            block.Add(assign);

            for(int i = 2; i < list.values.Count; i++)
            {
                block.Add(
                    Expression.Call(
                        arr, 
                        typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                         matchExpression(list.values[i], env)));
            }

            Expression type = unboxValue(aliasOrLiteralName(list.values[1], env), typeof(String));

            block.Add(Expression.Call(null, typeof(NetIneractLib).GetMethod("callConstruct"), 
                type, 
                Expression.Call(arr, typeof(List<ObjBox>).GetMethod("ToArray"))));
            return Expression.Block(new ParameterExpression[] { arr }, block);
        }

        private static Expression nullCheckExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 2)
                throw new ParsingException("Wrong number of arguments supplied to null?");

            ParameterExpression result = Expression.Parameter(typeof(ObjBox));

            Expression rhs = matchExpression(list.values[1], env);
            Expression type = Expression.Call(rhs, typeof(ObjBox).GetMethod("getType"));
            Expression listType = Expression.Call(null, typeof(TypeUtils).GetMethod("listType"));
            
            Expression test = Expression.Call(listType, typeof(Type).GetMethod("Equals", new Type[] { typeof(Type) }), type);

            Expression test2 = Expression.Call(unboxValue(rhs, typeof(RacketPair)), typeof(RacketPair).GetMethod("isNull"));

            Expression ifTrue = wrapInObjBox(
                Expression.Constant(true),
                Expression.Call(null, typeof(TypeUtils).GetMethod("boolType")));


            Expression ifFalse = wrapInObjBox(
                Expression.Constant(false),
                Expression.Call(null, typeof(TypeUtils).GetMethod("boolType")));

            Expression assignTrue = Expression.Assign(result, ifTrue);
            Expression assignFalse = Expression.Assign(result, ifFalse);

            Expression checkIfNull = Expression.IfThenElse(test2, assignTrue, assignFalse);
            Expression checkIfObjBox = Expression.IfThenElse(test, checkIfNull, assignFalse);

            return Expression.Block(new ParameterExpression[] { result }, new Expression[] { checkIfObjBox, result });
        }

        private static Expression nullList(Expression env)
        {
            Expression cons = Expression.New(
                typeof(RacketPair).GetConstructor(
                    new Type[] { }));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("pairType"));
            return wrapInObjBox(cons, type);
        }



        private static Expression cdrExpr(ListNode list, Expression env)
        {
            if(list.values.Count != 2)
            {
                throw new ParsingException("failed to parse cdr");
            }
            Expression pair = unboxValue(matchExpression(list.values[1], env), typeof(RacketPair));
            return Expression.Call(pair, typeof(RacketPair).GetMethod("cdr"));
        }

        private static Expression carExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 2)
            {
                throw new ParsingException("failed to parse car");
            }
            Expression pair = unboxValue(matchExpression(list.values[1], env), typeof(RacketPair));
            return Expression.Call(pair, typeof(RacketPair).GetMethod("car"));
        }

        private static Expression consExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 3)
                throw new ParsingException("Could not parse cons");

            Expression first = matchExpression(list.values[1], env);
            Expression rest = matchExpression(list.values[2], env);
            Expression cons = Expression.New(
                typeof(RacketPair).GetConstructor( 
                    new Type[] { typeof(ObjBox), typeof(ObjBox) }), 
                first, 
                rest);
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("pairType"));
            return wrapInObjBox(cons, type);
            
        }

        private static Expression setBangExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();

            //TODO check variable names are a legal scheme variable name
            if (list.values.Count != 3 || list.values[1].isList() || list.values[1].getValue().GetType() != typeof(String))
                throw new ParsingException("failed to parse set!");

            var name = Expression.Constant(list.values[1].getValue());

            var rhs = matchExpression(list.values[2], env);

            Expression set = Expression.Call(
                env,
                typeof(CompilerLib.Environment).GetMethod("set"),
                name,
                Expression.Convert(rhs, typeof(ObjBox)));

            block.Add(set);
            block.Add(voidSingleton);
            return Expression.Block(block);
        }

        private static Expression autoInvokeLambda(ListNode list, Expression env)
        {
            //first expression must be a lambda
            Expression lambBox = lambdaExpr((ListNode)list.values[0], env);

            //lets get the function wrapper out of the obj box
            Expression lamb = unboxValue(lambBox, typeof(FunctionHolder));

            List<Expression> invokeLamb = new List<Expression>();

            //Statement to make a List (it will hold the arguments)
            Expression newObjList = Expression.New(typeof(List<Object>).GetConstructor(new Type[] { }));
            ParameterExpression objList = Expression.Variable(typeof(List<Object>), "argList");
            Expression assignExpr = Expression.Assign(objList, newObjList);

            invokeLamb.Add(objList);
            invokeLamb.Add(assignExpr);

            // add each matched argument into our list of arguments
            for (int i = 1; i < list.values.Count; i++)
            {
                invokeLamb.Add(Expression.Call(
                    objList,
                    typeof(List<Object>).GetMethod("Add", new Type[] { typeof(Object) }),
                    Expression.Convert(matchExpression(list.values[i], env), typeof(Object))));
            }

            var invoke = Expression.Call(
                lamb,
                typeof(FunctionHolder).GetMethod("invoke"),
                objList);

            invokeLamb.Add(invoke);

            return Expression.Block(new ParameterExpression[] { objList }, invokeLamb);
        }

        private static Expression beginExpr(ListNode list, Expression env)
        {
            if (list.values.Count < 2)
                throw new ParsingException("empty body in a begin expression");

            List<Node> values = new List<Node>();

            for (int i = 1; i < list.values.Count; i++)
            {
                values.Add(list.values[i]);
            }
            ListNode bodyLiterals = new ListNode(values, 0, false);

            //TODO verify correctness of not making a new env
            return matchTopLevel(bodyLiterals, env);
        }

        private static Expression displayExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();

            if (list.values.Count != 2)
                throw new ParsingException("wrong number of arguments passed to displayln");

            Expression print = unboxValue(matchExpression(list.values[1], env), typeof(Object));
            Expression toStr = Expression.Call(print, typeof(Object).GetMethod("ToString"));
            Expression println = Expression.Call(null, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }), toStr);
            block.Add(println);
            block.Add(voidSingleton);

            return Expression.Block( block );
        }

        private static Expression equalExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse equals for list " + tree.ToString());

            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(Object));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(Object));

            //Expression lhs = matchExpression(tree.values[1], env);
            //Expression rhs = matchExpression(tree.values[2], env);

            Expression equal = Expression.Call(lhs, typeof(Object).GetMethod("Equals", new Type[] { typeof(Object) }), rhs);

            return Expression.New(
                    typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                    new Expression[] { 
                        Expression.Convert(equal, typeof(Object)), 
                        Expression.Call(null, typeof(TypeUtils).GetMethod("boolType")) });
        }

        private static Expression whileExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 3)
            {
                throw new ParsingException("Incorrect number of arguments supplied to While");
            }

            LabelTarget done = Expression.Label(typeof(ObjBox));
            Expression test = unboxValue(matchExpression(list.values[1], env), typeof(Boolean));
            Expression body = matchExpression(list.values[2], env);
            Expression fi = Expression.IfThenElse(test, body, Expression.Break(done, voidSingleton));

            return Expression.Block(new ParameterExpression[] { }, Expression.Loop(fi, done));
        }

        private static Expression ifExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 4)
                throw new ParsingException("wrong number of arguments for if expression");

            // this will get assigned the result of the if statement
            ParameterExpression objBox = Expression.Parameter(typeof(ObjBox));

            //one expression tree for each branch of the if statment
            Expression t = matchExpression(list.values[2], env);
            Expression f = matchExpression(list.values[3], env);

            ParameterExpression tTemp = Expression.Parameter(typeof(ObjBox));
            ParameterExpression fTemp = Expression.Parameter(typeof(ObjBox));

            //asign whichever branch we take to our return value, objBox
            Expression ifTrue =  Expression.Assign(objBox, Expression.Convert(t, typeof(ObjBox)));
            Expression ifFalse = Expression.Assign(objBox, Expression.Convert(f, typeof(ObjBox)));

            
            Expression ifThenElse = Expression.IfThenElse(
                unboxValue(matchExpression(list.values[1], env), typeof(Boolean)),
                ifTrue,
                ifFalse);

            return Expression.Block( new ParameterExpression[] {objBox, }, new Expression[] { ifThenElse, objBox });
        }

        private static Expression invokeLambda(ListNode tree, Expression env)
        {
            List<Expression> invokeLamb = new List<Expression>();

            //Statement to make a List
            Expression newObjList = Expression.New(typeof(List<Object>).GetConstructor(new Type[] {}));
            ParameterExpression objList = Expression.Variable(typeof(List<Object>), "argList");
            Expression assignExpr = Expression.Assign(objList, newObjList);

            invokeLamb.Add(objList);
            invokeLamb.Add(assignExpr);
            
            // add each matched argument into our list of arguments
            for (int i = 1; i < tree.values.Count; i++)
            {
                invokeLamb.Add(Expression.Call(
                    objList,
                    typeof(List<Object>).GetMethod("Add", new Type[] { typeof(Object) }),
                    Expression.Convert(matchExpression(tree.values[i], env), typeof(Object))));
            }

            Expression getFunction = unboxValue(lookup(Expression.Constant(tree.values[0].getValue()), env), typeof(FunctionHolder));
  
            var invoke = Expression.Call(
                getFunction,
                typeof(FunctionHolder).GetMethod("invoke"), 
                objList);

            invokeLamb.Add(invoke);

            return Expression.Block(new ParameterExpression[] {objList}, invokeLamb);
        }

        // create a new lambda expression and store it into the environment
        private static Expression lambdaExpr(ListNode tree, Expression env)
        {

            //make new environment
            var makeEnv = Expression.New(
                typeof(CompilerLib.Environment).GetConstructor(new Type[] { typeof(CompilerLib.Environment) }),
                env);
            var new_env = Expression.Variable(typeof(CompilerLib.Environment));
            var assign = Expression.Assign(new_env, makeEnv);

            //Add the environment to the start of the body of the lambda
            List<Expression> body = new List<Expression>();
            body.Add(new_env);
            body.Add(assign);

            //make our parameter list
            List<ParameterExpression> paramList = new List<ParameterExpression>();

            foreach (Node n in tree.values[1].getList())
            {
                if (!n.isLeaf())
                    throw new ParsingException("list embeded in parameter list");

                ParameterExpression var = Expression.Variable(typeof(Object), n.getValue()); 
                paramList.Add(var);

                // we also need to add a statement to put each of our parameters into our new environment (for scoping reasons)
                var name = Expression.Constant(n.getValue());

                Expression addToEnv = Expression.Call(
                    new_env,
                    typeof(CompilerLib.Environment).GetMethod("add"),
                    name,
                    Expression.Convert(var, typeof(ObjBox)));

                body.Add(addToEnv);

            }

            List<Node> values = new List<Node>();

            for (int i = 2; i < tree.values.Count; i++)
            {
                values.Add(tree.values[i]);
            }
            ListNode bodyLiterals = new ListNode(values, 0, false);

            //Add the body of the lambda to the expression tree
            body.Add(matchTopLevel(bodyLiterals, new_env));


            var lambda = Expression.Lambda(Expression.Block(new ParameterExpression[] { new_env }, body), paramList);

            //Now we want to box this expression in a FunctionHolder
            Expression func = Expression.New(
                typeof(FunctionHolder).GetConstructor(new Type[] { typeof(Delegate) }),
                lambda);

            //Now lets box this into a objBox
            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] {typeof(Object), typeof(Type)}),
                Expression.Convert(func, typeof(Object)),
                Expression.Call(null, typeof(TypeUtils).GetMethod("funcType")));
        }

        private static Expression divExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse plus for list " + tree.ToString());

            //unboxing from type object
            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
            Expression result = Expression.Divide(lhs, rhs);

            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                new Expression[] { Expression.Convert(result, typeof(Object)), type });
        }

        private static Expression modExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse plus for list " + tree.ToString());

            //unboxing from type object
            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
            Expression result = Expression.Modulo(lhs, rhs);

            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                new Expression[] { Expression.Convert(result, typeof(Object)), type });
        }

        private static Expression multExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse plus for list " + tree.ToString());

            //unboxing from type object
            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
            Expression result = Expression.Multiply(lhs, rhs);

            return  Expression.New(
                    typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                    new Expression[] { Expression.Convert(result, typeof(Object)), type });
        }

        private static Expression addExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse plus for list " + tree.ToString());

            //unboxing from type object
            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));

            Expression result = Expression.Add(lhs, rhs);
            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                new Expression[] { Expression.Convert(result, typeof(Object)), type });
        }

        private static Expression subExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
                throw new ParsingException("failed to parse plus for list " + tree.ToString());

            //unboxing from type object
            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));

            Expression result = Expression.Subtract(lhs, rhs);
            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                new Expression[] { Expression.Convert(result, typeof(Object)), type });
        }

        private static Expression defineExpr(ListNode tree, Expression env)
        {
            List<Expression> block = new List<Expression>();
            
            //TODO check variable names are a legal scheme variable name
            if (tree.values.Count != 3 || tree.values[1].isList() || tree.values[1].getValue().GetType() != typeof(String))
                throw new ParsingException("failed to parse define");

            var rhs = matchExpression(tree.values[2], env);

            var name = Expression.Constant(tree.values[1].getValue());

            Expression add = Expression.Call(
                env,
                typeof(CompilerLib.Environment).GetMethod("add"),
                name,
                Expression.Convert(rhs, typeof(ObjBox)));

            block.Add(add);
            block.Add(voidSingleton);
            return Expression.Block(block);
        }


        //unboxes the object allowing us to safley cast it at runtime
        static Expression unboxValue(Expression obj, Type type)
        {
            List<Expression> body = new List<Expression>();

            ParameterExpression temp = Expression.Parameter(typeof(ObjBox));
            Expression Assign = Expression.Assign(temp, Expression.Convert(obj, typeof(ObjBox)));

            body.Add(Assign);

            Expression castMethod = Expression.Call(temp, typeof(ObjBox).GetMethod("getConv"));

            var objArray = Expression.NewArrayInit(
                typeof(Object),
                Expression.Call(temp, typeof(ObjBox).GetMethod("getObj")));

           Expression invoke = Expression.Call(
                castMethod, 
                typeof(System.Reflection.MethodInfo).GetMethod("Invoke", new Type[] { typeof(Object), typeof(Object[]) }),
                temp,
                objArray);

            Expression cast = Expression.Convert(invoke, type);
            body.Add(cast);
            return Expression.Block(new ParameterExpression[] { temp }, body);
        }

        static Expression wrapInObjBox(Expression obj, Expression type)
        {
            return Expression.New(
                    typeof(ObjBox).GetConstructor(new Type[] { typeof(Object), typeof(Type) }),
                    new Expression[] { Expression.Convert(obj, typeof(Object)), type });
        }

        //DEPRECIATED DO NOT USE
        //Using reflection we dynamically generate a templated call to a method that lets us recover the type of any object at runtime
        static Expression recoverTypeFromObjBox(Expression obj, Expression type, Expression env)
        {
            var envType = Expression.Call(
                env,
                typeof(TypeUtils).GetMethod("GetType"));

            var getMethodInfo = Expression.Call(
                envType,
                typeof(Type).GetMethod("GetMethod", new Type[] { typeof(String) }),
                Expression.Constant("cast"));

            var typeArray = Expression.NewArrayInit(
                typeof(Type),
                type);

            var makeGeneric = Expression.Call(
                getMethodInfo,
                typeof(MethodInfo).GetMethod("MakeGenericMethod"),
                typeArray);

            var objArray = Expression.NewArrayInit(
                typeof(Object),
                obj);

            var unbox = Expression.Call(
                makeGeneric,
                typeof(MethodInfo).GetMethod("Invoke", new Type[] { typeof(Object), typeof(Object[]) }),
                obj,
                objArray);

            return unbox;
        }

        //Uses gets an objBox from the environment
        static Expression lookup(Expression str, Expression env)
        {
            //Perform a variable lookup
            var lookup = Expression.Call(
                env,
                typeof(CompilerLib.Environment).GetMethod("lookup", new Type[] { typeof(String) }),
                str);
            /*
            var type = Expression.Call(
                env,
                typeof(Environment).GetMethod("lookupType", new Type[] { typeof(String) }),
                Expression.Constant(str));
            */
            return lookup;
            //return accessValue(lookup);
        }

        static Expression checkEnv(Expression name, Expression env)
        {
            Expression check = Expression.Call(
                env,
                typeof(CompilerLib.Environment).GetMethod("check"),
                name);

            return check;
        }

        static Expression matchLiteralList(ListNode tree, Expression env)
        {

            if (tree.values.Count == 0)
            {
                return voidSingleton;
            }

            Expression first;
            Expression rest;

            Node n = tree.values[0];
            tree.values.RemoveAt(0);
            rest = matchLiteralList(tree, env);
            
            if (n.isList()) // check if literal list
            {
                first = matchLiteralList((ListNode) n, env);
            }
            else // is atom
            {
                bool matchedAtom;
                first = matchAtom('\'' + n.getValue(), out matchedAtom);
                if(!matchedAtom)
                    throw new RuntimeException("Could not parse literal list");
            }

            Expression cons = Expression.New(
                typeof(RacketPair).GetConstructor(
                    new Type[] { typeof(ObjBox), typeof(ObjBox) }),
                first,
                rest);
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("pairType"));
            return wrapInObjBox(cons, type);
        }

        static Expression matchLeaf(Node leaf, Expression env)
        {

            bool matchedAtom;
            Expression e;
            e = matchAtom(leaf.getValue(), out matchedAtom);
            if (matchedAtom)
            {
                return e;
            }
            else
            {
                return lookup(Expression.Constant(leaf.getValue()), env);
            }
        }

        // matches an atom returning a constant expression
        static Expression matchAtom(String value, out bool isAtom)
        {
            
            Expression matchedExpr = null;
            isAtom = false;
            int number;

            if (value == "#t")
            {
                
                // this expression is the same as calling typeof(Boolean);
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));
                matchedExpr = wrapInObjBox(Expression.Constant(true), type);
                isAtom = true;
            }
            else if (value == "#f")
            {
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));
                matchedExpr = wrapInObjBox(Expression.Constant(false), type);
                isAtom = true;
            }
            else if (Int32.TryParse(value, out number))
            {
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
                matchedExpr = wrapInObjBox(Expression.Constant(int.Parse(value)), type);
                isAtom = true;
            }
            else if (value[0] == '\"')
            {
                value = value.Substring(1);
                value = value.Substring(0, value.Length);

                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("strType"));
                matchedExpr = wrapInObjBox(Expression.Constant(value, typeof(String)), type);
                isAtom = true;
            }
            else if (value[0] == '\'')
            {
                if (Int32.TryParse(value.Substring(1), out number))
                {
                    Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
                    matchedExpr = wrapInObjBox(Expression.Constant(int.Parse(value.Substring(1))), type);
                    isAtom = true;
                }
                else // string case
                {
                    Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("strType"));
                    matchedExpr = wrapInObjBox(Expression.Constant(value.Substring(1), typeof(String)), type);
                    isAtom = true;
                }
            }
            else if (value == "void" || value == "(void)")
            {
                isAtom = true;
                matchedExpr = voidSingleton;
            }

            return matchedExpr;
        }

        static void PrintHelp()
        {
            Console.WriteLine("if this were a real project then we would print a help document here");
        }
    }
}