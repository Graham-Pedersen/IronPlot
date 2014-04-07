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
using System.Numerics;
using CompilerLib;


namespace DLR_Compiler
{
   public class DLR_Compiler
    {
        static ParameterExpression voidSingleton;

        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception("Compiler called with incorrect number of arguments!");
            }
            //string filename = @"C:\Users\graha_000\Programing\IronPlot\test\12.plot";
            string filename = args[0];
            //Console.WriteLine("Compiling file " + filename);

            string mode = args[1];
            string outputName = args[2];

            //set up a single instance of type void
            voidSingleton = Expression.Variable(typeof(ObjBox), "void");
            Expression voidType = Expression.Call(null, typeof(TypeUtils).GetMethod("voidType"));


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


            //Body of the program
            List<Expression> program = new List<Expression>();

            //TODO
            //Add the link to Compiler_lib
            //String dllLoc = Directory.GetCurrentDirectory() + "\\" + "Compiler_Lib.dll";
            //Expression usingCompilerLib = Expression.Call(null, typeof(typeResolver).GetMethod("import"), Expression.Constant(dllLoc));
            //program.Add(usingCompilerLib);

            //Add the environment to the start of the program
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
                var asmName = new AssemblyName(outputName.Remove(outputName.IndexOf(".exe")));
                var asmBuilder = AssemblyBuilder.DefineDynamicAssembly
                    (asmName, AssemblyBuilderAccess.RunAndSave);
                var moduleBuilder = asmBuilder.DefineDynamicModule(outputName.Remove(outputName.IndexOf(".exe")), outputName);
                var typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Public);
                var methodBuilder = typeBuilder.DefineMethod("Main",
                    MethodAttributes.Static, typeof(void), new[] { typeof(string) });

                Expression.Lambda<Action>(code).CompileToMethod(methodBuilder);

                typeBuilder.CreateType();
                asmBuilder.SetEntryPoint(methodBuilder);
                asmBuilder.Save(outputName);
            }
            if (mode == "run")
            {
                Expression.Lambda<Action>(code).Compile()();
                    
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
                    List<Expression> body = new List<Expression>();
                    foreach(Node n in list.values)
                    {
                        body.Add(matchTopLevel(n, env));
                    }
                    return Expression.Block(
                            new ParameterExpression[] { },
                            body);
                }
                if (list.values[0].isLeaf())
                {
                    Expression name = Expression.Constant(list.values[0].getValue(), typeof(String));
                    Expression check = checkEnv(name, env);
                    Expression defaultExpression = null;
                    Expression envDefined = invokeLambda(list, env);

                    if (list.values[0].isLeaf() && list.values[0].getValue() == "define")
                    {
                        defaultExpression = defineExpr(list, env);
                        return envOverrideBranch(check, envDefined, defaultExpression);
                    }
                    else if (list.values[0].isLeaf() && list.values[0].getValue() == "using")
                    {
                        defaultExpression = netImportStmt(list, env);
                        return envOverrideBranch(check, envDefined, defaultExpression);
                    }
                }
            }
            return matchExpression(tree, env);
        }


        // This matches a list and converts it into an expression
        static Expression matchExpression(Node tree, Expression env)
        {
            if (tree.isLeaf())
            {
                //match on the leaf nodes
                return matchLeaf((LeafNode) tree, env);
            }

            ListNode list = (ListNode)tree;

            if (list.values.Count == 0)
            {
                return voidSingleton;
            }

            //we know if the first part is another list we are defining a lambda to be immediately 
            if (list.values[0].isList())
            {
                return invokeLambda(list, env);
            }
            else
            {
                Expression name = Expression.Constant(list.values[0].getValue(), typeof(String));
                Expression check = checkEnv(name, env);
                Expression defaultExpression = null;
                //Expression envDefined = invokeLambda(list, env);


                switch (list.values[0].getValue())
                {
                    //TODO REMOVE THIS AFTER DESUGARER FIX (add using as a top level form that escapes transformation)
                    case "using":
                        defaultExpression = netImportStmt(list, env);
                        break;

                    case "call":
                        defaultExpression = callNetExpr(list, env);
                        break;

                    case "scall":
                        defaultExpression = scallNetExpr(list, env);
                        break;

                    case "new":
                        defaultExpression = newNetObj(list, env);
                        break;

                    case "typelist":
                        defaultExpression = newTypeList(list, env);
                        break;

                    case "<":
                        defaultExpression = lessThanExpr(list, env);
                        break;

                    case "<=":
                        defaultExpression = lessThanEqualExpr(list, env);
                        break;

                    case ">":
                        defaultExpression = gretThanExpr(list, env);
                        break;

                    case ">=":
                        defaultExpression = gretThanEqualExpr(list, env);
                        break;

                    case "+":
                        defaultExpression = addExpr(list, env);
                        break;

                    case "-":
                        defaultExpression = subExpr(list, env);
                        break;

                    case "*":
                        defaultExpression = multExpr(list, env);
                        break;

                    case "/":
                        defaultExpression = divExpr(list, env);
                        break;

                    case "%":
                        defaultExpression = modExpr(list, env);
                        break;

                    case "void":
                        defaultExpression = voidSingleton;
                        break;

                    case "equal?":
                        defaultExpression = equalExpr(list, env);
                        break;

                    case "not":
                        defaultExpression = notExpr(list, env);
                        break;

                    case "lambda":
                        defaultExpression = lambdaExpr(list, env);
                        break;

                    case "if":
                        defaultExpression = ifExpr(list, env);
                        break;

                    case "while":
                        defaultExpression = whileExpr(list, env);
                        break;

                    case "set!":
                        defaultExpression = setBangExpr(list, env);
                        break;

                    case "cons":
                        defaultExpression = consExpr(list, env);
                        break;

                    case "car":
                        defaultExpression = carExpr(list, env);
                        break;

                    case "cdr":
                        defaultExpression = cdrExpr(list, env);
                        break;

                    case "begin":
                        defaultExpression = beginExpr(list, env);
                        break;

                    case "begin0":
                        defaultExpression = begin0Expr(list, env);
                        break;

                    case "displayln":
                        defaultExpression = displayExpr(list, env);
                        break;

                    case "null?":
                        defaultExpression = nullCheckExpr(list, env);
                        break;

                    case "map":
                        defaultExpression = mapExpr(list, env);
                        break;

                    case "quote":
                        if (list.values.Count != 2)
                        {
                            defaultExpression = createRuntimeException("wrong number of arguments passed to quote procedure");
                        }
                        else
                        {
                            defaultExpression = matchLiteral(list.values[1], env);
                        }
                        break;

                    //TODO add environment check and move above standard cases
                    default:
                        defaultExpression = createRuntimeException("Could not resolve procedure:" + list.values[0].getValue());
                        break;
                }

                return defaultExpression;
                //return envOverrideBranch(check, envDefined, defaultExpression);
            }
        }

        private static Expression envOverrideBranch(Expression envCheck, Expression envDef, Expression builtInDef)
        {
            ParameterExpression result = Expression.Parameter(typeof(ObjBox));

            Expression branch = Expression.IfThenElse(
                envCheck,
                Expression.Assign(result, envDef),
                Expression.Assign(result, builtInDef));

            return Expression.Block(
                new ParameterExpression[] { result },
                new Expression[] { branch, result });
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
            {
                 return createRuntimeException("wrong number of arguments passed to not procedure");
            }

            Expression not = Expression.Not(unboxValue(matchExpression(list.values[1], env), typeof(Boolean)));
            return wrapInObjBox(
                not,
                Expression.Call(null, typeof(TypeUtils).GetMethod("boolType")));
        }

        private static Expression netImportStmt(ListNode list, Expression env)
        {
            if(list.values.Count != 2)
            {
                return createRuntimeException("wrong number of arguments passed to using procedure");
            }

            Expression importStr = Expression.Constant(list.values[1].getValue());
            return Expression.Block(
            new ParameterExpression[] { }, 
            new Expression[]  { Expression.Call(null, typeof(typeResolver).GetMethod("import", new Type[] { typeof(String) }), importStr) , voidSingleton });
        }

        private static Expression scallNetExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();
            if (list.values.Count < 3)
            {
                return createRuntimeException("wrong number of arguments passed to static-call procedure");
            }

            ParameterExpression arr = Expression.Parameter(typeof(List<ObjBox>));
            Expression argArray = Expression.New(typeof(List<ObjBox>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(arr, argArray);
            block.Add(assign);

            
            Expression arg;
            if (list.values.Count > 3)
            {
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
                arg = matchExpression(list.values[i], env);
                block.Add(
                    Expression.Call(
                        arr,
                        typeof(List<ObjBox>).GetMethod("Add", new Type[] { typeof(ObjBox) }),
                         matchExpression(list.values[i], env)));
            }
            Expression name = aliasOrLiteralName(list.values[1], env);
            Expression target = aliasOrLiteralName(list.values[2], env);

            Expression typeStr = unboxValue(name, typeof(String));
            Expression targetStr = unboxValue(target, typeof(String));

            block.Add(Expression.Call(null, typeof(NetIneractLib).GetMethod("call"),
                wrapInObjBox(typeStr, Expression.Call(null, typeof(TypeUtils).GetMethod("voidType"))),
                targetStr,
                Expression.Call(arr, typeof(List<ObjBox>).GetMethod("ToArray"))));

            return Expression.Block(new ParameterExpression[] { arr }, block);
        }

        private static Expression callNetExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();
            if (list.values.Count < 3)
            {
                return createRuntimeException("wrong number of arguments passed to call procedure");
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
            Expression name = aliasOrLiteralName(list.values[2], env);
            Expression callStr = unboxValue(name, typeof(String));
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
                return createRuntimeException("wrong number of arguments passed to new procedure");
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
            {
                return createRuntimeException("wrong number of arguments passed to null? procedure");
            }

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
                return createRuntimeException("wrong number of arguments passed to cdr procedure");
            }
            Expression pair = unboxValue(matchExpression(list.values[1], env), typeof(RacketPair));
            return Expression.Call(pair, typeof(RacketPair).GetMethod("cdr"));
        }

        private static Expression carExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 2)
            {
                return createRuntimeException("wrong number of arguments passed to car procedure");
            }
            Expression pair = unboxValue(matchExpression(list.values[1], env), typeof(RacketPair));
            return Expression.Call(pair, typeof(RacketPair).GetMethod("car"));
        }

        private static Expression consExpr(ListNode list, Expression env)
        {
            if (list.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to cons procedure");
            }

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
            {
                return createRuntimeException("wrong number of arguments passed to set! procedure");
            }

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
            {
                return createRuntimeException("wrong number of arguments passed to begin procedure");
            }

            List<Node> values = new List<Node>();

            for (int i = 1; i < list.values.Count; i++)
            {
                values.Add(list.values[i]);
            }
            ListNode bodyLiterals = new ListNode(values, 0, false);

            return matchTopLevel(bodyLiterals, env);
        }

        private static Expression begin0Expr(ListNode list, Expression env)
        {
            if (list.values.Count < 2)
            {
                return createRuntimeException("wrong number of arguments passed to begin procedure");
            }

            List<Node> values = new List<Node>();

            return null;
        }

        private static Expression foldlExpr(ListNode list, Expression env)
        {
            int foldCount = list.values.Count;
            if (foldCount < 4)
            {
                return createRuntimeException("wrong number of arguments passed to foldl procedure");
            }

            // 1. function is user defined
            // 2. function is built in
            // 3. function is a lambda

            Node function = list.values[1];
            Expression fun;
            List<Expression> body = new List<Expression>();

            // init lists for map
            ParameterExpression listArrs = Expression.Parameter(typeof(List<RacketPair>));
            Expression argArray = Expression.New(typeof(List<RacketPair>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(listArrs, argArray);

            // must create parameter for init argument, don't know type
            // TODO

            body.Add(assign);
            // must add parameter for init arg

            if (function.isList()) // lambda case 
            {
                ListNode l = (ListNode)function;
                if (!l.values[0].getValue().Equals("lambda")) // should be lambda, just check that it is
                    throw new ParsingException("not a lambda, but should be");
                if (l.values.Count != 3)
                    throw new ParsingException("bad syntax, lambda");
                ListNode parameters = (ListNode)l.values[1]; // parameters,
                int count = parameters.values.Count;
                if (count == 0)
                    throw new ParsingException("number of parameters does not match number of lists given");
                int listCount = foldCount - 2;
                if (listCount != count)
                    throw new ParsingException("number of lists does not match expected number of arguments");

                fun = unboxValue(lambdaExpr(l, env), typeof(FunctionHolder));
            }
            else // built in or user defined case
            {
                fun = unboxValue(lookup(Expression.Constant(function.getValue()), env), typeof(FunctionHolder));
                // graham will fix this by changing lookup, so if it fails its his fault
            }

            return null;
        }

        private static Expression mapExpr(ListNode list, Expression env)
        {
            int mapCount = list.values.Count;
            if (mapCount < 3)
            {
                return createRuntimeException("wrong number of arguments passed to map procedure");
            }

            // 1. function is user defined
            // 2. function is built in
            // 3. function is a lambda

            Node function = list.values[1];
        //    Expression lambda;
            Expression fun;
            List<Expression> body = new List<Expression>();
            
            // init for lists from map
            ParameterExpression arr = Expression.Parameter(typeof(List<RacketPair>));
            Expression argArray = Expression.New(typeof(List<RacketPair>).GetConstructor(new Type[] { }));
            Expression assign = Expression.Assign(arr, argArray);
            body.Add(assign);

            if (function.isList()) // lambda case 
            {
                ListNode l = (ListNode)function;
                if (!l.values[0].getValue().Equals("lambda")) // should be lambda, just check that it is
                    throw new ParsingException("not a lambda, but should be");
                if (l.values.Count != 3)
                    throw new ParsingException("bad syntax, lambda");
                ListNode parameters = (ListNode)l.values[1]; // parameters,
                int count = parameters.values.Count;
                if (count == 0)
                    throw new ParsingException("number of parameters does not match number of lists given");
                int listCount = mapCount - 2;
                if (listCount != count)
                    throw new ParsingException("number of lists does not match expected number of arguments");

                fun = unboxValue(lambdaExpr(l, env), typeof(FunctionHolder));
            }
            else // built in or user defined case
            {
                fun = unboxValue(lookup(Expression.Constant(function.getValue()), env), typeof(FunctionHolder));
                // graham will fix this by changing lookup, so if it fails its his fault
            }

            for (int h = 2; h < mapCount; h++)
            {
                Expression l_ = unboxValue(matchExpression(list.values[h], env), typeof(RacketPair));
                body.Add(
                    Expression.Call(
                    arr, 
                    typeof(List<RacketPair>).GetMethod("Add", new Type[] { typeof(RacketPair) }), 
                    l_)); 
            }

            body.Add(Expression.Call(null,
                typeof(FunctionLib).GetMethod("Map"), fun, arr));

            return Expression.Block(new ParameterExpression[] { arr }, body);
            
        }

        private static Expression displayExpr(ListNode list, Expression env)
        {
            List<Expression> block = new List<Expression>();

            if (list.values.Count != 2)
            {
                return createRuntimeException("wrong number of arguments passed to displayln procedure");
            }

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
            {
                return createRuntimeException("wrong number of arguments passed to equal? procedure");
            }
        

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
                return createRuntimeException("wrong number of arguments passed to while procedure");
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
            {
                return createRuntimeException("wrong number of arguments passed to if procedure");
            }

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

            Expression getFunction = unboxValue(matchExpression(tree.values[0], env), typeof(FunctionHolder));
  
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
            if (tree.values.Count < 3 || !tree.values[1].isList())
            {
                return createRuntimeException("too few arguments passed to lambda procedure or first argument was not parameter list");
            }
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
                {
                    return createRuntimeException("list embeded in the parameter list of labmda expression");
                }

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
                typeof(FunctionHolder).GetConstructor(new Type[] { typeof(Delegate), typeof(int)}),
                lambda, Expression.Constant(paramList.Count, typeof(int)));
           
            //Now lets box this into a objBox
            return Expression.New(
                typeof(ObjBox).GetConstructor(new Type[] {typeof(Object), typeof(Type)}),
                Expression.Convert(func, typeof(Object)),
                Expression.Call(null, typeof(TypeUtils).GetMethod("funcType")));
        }

        private static Expression divExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to divide procedure");
            }

            //unboxing from type object
            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(RacketNum));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Div"), new Expression[] { rhs });
            return result;
        }

        private static Expression modExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to mod procedure");
            }

            //unboxing from type object
            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(RacketNum));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Mod"), new Expression[] { rhs });
            return result;
        }

        private static Expression multExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to multiply procedure");
            }

            //unboxing from type object
            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(RacketNum));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(RacketNum));

            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Mult"), new Expression[] { rhs });
            return result;
        }

        private static Expression addExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to add procedure");
            }

            //unboxing from type object
            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(RacketNum));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(RacketNum));
            
            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Plus"), new Expression[] { rhs });

            // we do not need to wrap this in an obj box because this is done in logic already
            //return wrapInObjBox(result, type);
            return result;
        }

        private static Expression subExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to subtract procedure");
            }

            //unboxing from type object
            Expression lhs = unboxValue(matchExpression(tree.values[1], env), typeof(RacketNum));
            Expression rhs = unboxValue(matchExpression(tree.values[2], env), typeof(RacketNum));

            
            Expression result = Expression.Call(lhs, typeof(RacketNum).GetMethod("Sub"), new Expression[] { rhs });
            return result;
        }

        private static Expression lessThanExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to less-than procedure");
            }

            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));

            Expression result = Expression.LessThan(lhs, rhs);

            return wrapInObjBox(result, type);

        }

        private static Expression lessThanEqualExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to less-than-equal procedure");
            }

            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));

            Expression result = Expression.LessThanOrEqual(lhs, rhs);

            return wrapInObjBox(result, type);

        }


        private static Expression gretThanExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to greater-than procedure");
            }

            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));

            Expression result = Expression.GreaterThan(lhs, rhs);

            return wrapInObjBox(result, type);
        }

        private static Expression gretThanEqualExpr(ListNode tree, Expression env)
        {
            if (tree.values.Count != 3)
            {
                return createRuntimeException("wrong number of arguments passed to greater-than-equal procedure");
            }

            dynamic lhs = unboxValue(matchExpression(tree.values[1], env), typeof(int));
            dynamic rhs = unboxValue(matchExpression(tree.values[2], env), typeof(int));
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));

            Expression result = Expression.GreaterThanOrEqual(lhs, rhs);

            return wrapInObjBox(result, type);
        }

        private static Expression defineExpr(ListNode tree, Expression env)
        {
            List<Expression> block = new List<Expression>();
            
            //TODO check variable names are a legal scheme variable name
            if (tree.values.Count != 3 || tree.values[1].isList() || tree.values[1].getValue().GetType() != typeof(String))
            {
                return createRuntimeException("define procedure failed");
            }

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


        //unboxes the object allowing us to safley  perfomar a cast at runtime
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
                typeof(CompilerLib.Environment).GetMethod("check", new Type[] { typeof(String) }),
                name);

            return check;
        }

        

       //If the value of the leaf node is symbol held by the environment return that
       //else try to treat the value as an atomic
        static Expression matchLeaf(LeafNode leaf, Expression env)
        {

            Expression name = Expression.Constant(leaf.getValue());
            Expression check = checkEnv(name, env);

            Expression envlookup = lookup(name, env);
            Expression atom = matchAtom(leaf.getValue());

            return envOverrideBranch(check, envlookup, atom);
        } 


        static Expression matchLiteral(Node lit, Expression env)
        {
            if (lit.isList())
            {
                return matchLiteralList((ListNode) lit, env);
            }

            String value = ((LeafNode) lit).getValue();

            if (isBoolean(value))
                return parseBoolean(value);
            if (isNumber(value))
                return parseNumber(value);

            Expression type = Expression.Call(typeof(TypeUtils).GetMethod("strType"));
            return wrapInObjBox(Expression.Constant(value, typeof(String)), type);


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
            first = matchLiteral(n, env);

            tree.values.RemoveAt(0);
            rest = matchLiteral(tree, env);
           
            Expression cons = Expression.New(
                typeof(RacketPair).GetConstructor(
                    new Type[] { typeof(ObjBox), typeof(ObjBox) }),
                first,
                rest);
            Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("pairType"));
            return wrapInObjBox(cons, type);
        }


        // matches an atom returning a constant expression
        // Atoms can be a number type or a boolean type
        static Expression matchAtom(String atom)
        {

            if (isBoolean(atom))
            {
                return parseBoolean(atom);
            }

            if (isNumber(atom))
            {
                return parseNumber(atom);
            }

            return createRuntimeException("value was not defined and could not be parsed as an atomic value:" + atom);
        }

        static Boolean isBoolean(String atom)
        {
            Expression result = parseBoolean(atom);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        static Expression parseBoolean(String atom)
        {
            if (atom == "#t")
            {
                // this expression is the same as calling typeof(Boolean);
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));
                return wrapInObjBox(Expression.Constant(true), type);
            }

            if (atom == "#f")
            {
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("boolType"));
                return wrapInObjBox(Expression.Constant(false), type);
            }
            return null;
        }

        static Boolean isNumber(String atom)
        {
            Expression result = parseNumber(atom);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        static Expression parseNumber(String atom)
        {
            int num;
            Double flo;
            //TODO add complex numbers

            if (Int32.TryParse(atom, out num))
            {
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("intType"));
                Expression numCons = Expression.New(
                    typeof(RacketInt).GetConstructor(new Type[] { typeof(int) }),
                    Expression.Constant(num));

                return wrapInObjBox(numCons, type);
            }
            
            if (Double.TryParse(atom, out flo))
            {
                Expression type = Expression.Call(null, typeof(TypeUtils).GetMethod("floatType"));
                Expression numCons = Expression.New(
                  typeof(RacketFloat).GetConstructor(new Type[] { typeof(Double) }),
                  Expression.Constant(flo));

                return wrapInObjBox(numCons, type);
            }

            return null;
        }

        static Expression createRuntimeException(String message)
        {
           return Expression.Block(
                    new ParameterExpression[] { },
                    new Expression[] {
                        Expression.Throw(
                            Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(String) }),
                            Expression.Constant(message, typeof(String)))), 
                        voidSingleton});
        }

        static void PrintHelp()
        {
            Console.WriteLine("if this were a real project then we would print a help document here");
        }
    }
}