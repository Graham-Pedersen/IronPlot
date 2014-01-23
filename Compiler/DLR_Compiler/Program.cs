using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SimpleSchemeParser;

namespace DLR_Compiler
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }
            String filename = args[0];
            Console.WriteLine("Compiling file " + args[0]);
            
            // make a new simple scheme parser
            SchemeParser ssp = new SchemeParser(filename);
            ListNode topLevelForms = ssp.parseFile();
                    
    
            // these expressions will initalize the top level environment
            Expression makeEnv = Expression.New(typeof(Environment));
            var env = Expression.Variable(typeof(Environment), "env");
            Expression assign = Expression.Assign(env, makeEnv);

                    
            //Add the environment to the start of the program
            List<Expression> program = new List<Expression>();
            program.Add(env);
            program.Add(assign);

            //Match and add the rest of the program
            program.Add(matchTopLevel(topLevelForms, env));

            //Wrap the program into a block expression
            Expression code = Expression.Block(new ParameterExpression[] {env}, program);

            //Put the block expression into a lambda function and invoke it
            Console.WriteLine(Expression.Lambda<Func<int>>(code).Compile()());

            //TODO change to either output into .exe .dll or invoke automatically (interpret)
            Console.ReadKey();
        }

       
        /** match all top level forms in the form of:
         * 
         *  topLevelForms ::= (<def> | <exp>)
         *
         **/
        static Expression matchTopLevel(Node tree, Expression env)
        {
            // currently we ju
            if (tree.isList())
            {
                ListNode list = (ListNode)tree;
                if (list.values[0].isLeaf() && list.values[0].getValue() == "define" )
                {
                    //TODO fix type system
                    //TODO check variable names are a legal scheme variable name
                    if (list.values.Count != 3 || list.values[1].isList() || list.values[1].getValue().GetType() != typeof(String))
                        throw new ParsingException("failed to parse define");

                    Expression call = Expression.Call(
                        env,
                        typeof(Environment).GetMethod("add", new Type[] { typeof(String), typeof(int) }),
                        Expression.Constant(list.values[1].getValue()),
                        match(list.values[2], env));

                    return call;
                }
            }
            return match(tree, env);
        }

        static Expression match(Node tree, Expression env)
        {
            // Match atoms
            if (tree.isLeaf())
            {
                return matchLeaf(tree, env);
            }
            // Match a non atom statement
            else
            {
                ListNode list = (ListNode) tree;

                if (list.values[0].isLeaf())
                {
                    return matchExpression((ListNode) tree, env);               
                }
                // if we have a list of lists we either have a body or a top level form...
                else
                {
                    List<Expression> dlrTree = new List<Expression>(); 
                    foreach (Node n in list.getList())
                    {
                        dlrTree.Add(matchTopLevel(n, env));
                    }
                    return Expression.Block(dlrTree);
                }
            }
        }

        // This matches an expression of some type
        static Expression matchExpression(ListNode tree, Expression env)
        {
            switch (tree.values[0].getValue())
            {

                case "+":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse plus for list " + tree.ToString());
                    return Expression.Add(match(tree.values[1], env), match(tree.values[2], env));

                case "-":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse minus for list " + tree.ToString());
                    return Expression.Subtract(match(tree.values[1], env), match(tree.values[2], env));

                case "*":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse times for list " + tree.ToString());
                    return Expression.Multiply(match(tree.values[1], env), match(tree.values[2], env));

                case "/":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse divide for list " + tree.ToString());
                    return Expression.Divide(match(tree.values[1], env), match(tree.values[2], env));

                case "%":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse mod for list " + tree.ToString());
                    return Expression.Modulo(match(tree.values[1], env), match(tree.values[2], env));

                case "equals":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse equals for list " + tree.ToString());
                    return Expression.Equal(match(tree.values[1], env), match(tree.values[2], env));

                case "if":
                    throw new NotImplementedException();

                case "lambda":  
                    throw new NotImplementedException();

                case "cons":
                    throw new NotImplementedException();

                case "car": 
                    throw new NotImplementedException();

                case "cdr":
                    throw new NotImplementedException();

            }
            throw new ParsingException("could not match patern: " + tree.values[0].ToString());
        }

        static Expression matchLeaf(Node leaf, Expression env)
        {

            bool matchedAtom;
            Expression e;
            e = matchAtom(leaf, out matchedAtom);
            if (matchedAtom)
            {
                // the tree is a atom leaf 
                return e;
            }
            else
            {
                //Perform a variable lookup
                return Expression.Block(
                            Expression.Call(
                            env,
                            typeof(Environment).GetMethod("lookup", new Type[] { typeof(String)}),
                            Expression.Constant(leaf.getValue())));
            }
        }

        // matches an atom returning a constant expression
        static Expression matchAtom(Node atom, out bool isAtom)
        {
           
            String value = atom.getValue();
            if (value == "#t")
            {
                isAtom = true;
                return Expression.Constant(true, typeof(bool));
            }
            if (value == "#f")
            {
                isAtom = true;
                return Expression.Constant(false, typeof(bool));
            }
            int number;
            if (Int32.TryParse(value, out number))
            {
                isAtom = true;
                return Expression.Constant(int.Parse(value), typeof(int));
            }
            //TODO make this understand how scheme does litearal lists aka '(blah blag) vs 'blah
            if (value[0] == '\'')
            {
                isAtom = true;
                return Expression.Constant(value, typeof(String));
            }
            //TODO add support for void

            isAtom = false;
            return null;

        }

        static void PrintHelp()
        {
            Console.WriteLine("if this were a real project then we would print a help document here");
        }
    }
}
