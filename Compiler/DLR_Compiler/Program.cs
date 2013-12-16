using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DLR_Compiler
{
    class Program
    {

        //Regex patterns
        //static String patternWhitespace = "[ \t\r\n]";
        static String patternNotWhitespaceOrParen = "[^ \t$()\r\n]";
        static String patternMatchAtom = "[ \t\r\n]*[^ ()\t\r\n]+[ \t\r\n]*";
        static String patternMatchAtomNoWhiteSpace = "[^ \t()\r\n]*";

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }
            String filename = args[0];
            Console.WriteLine("Compiling file " + args[0]);
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    ListNode topLevelForms = captureList(sr.ReadToEnd(), 0).Item1;
                    
                    //topLevelForms.print();

                    Expression makeEnv = Expression.New(typeof(Environment));
                    ParameterExpression env = Expression.Variable(typeof(Environment), "env");
                    Expression assign = Expression.Assign(env, makeEnv);

                    List<Expression> program = new List<Expression>();
                    

                    Expression code = Expression.Block(new ParameterExpression[] { env }, new Expression[] {assign, match(topLevelForms)});

                    Console.WriteLine(Expression.Lambda<Func<int>>(code).Compile()());
                }
            }
            catch (EndOfStreamException)
            {
                throw new FileNotFoundException("File: does not exist");
            }
            Console.ReadKey();
        }

        static Expression match(Node tree)
        {
            List<Expression> expressionList = new List<Expression>();

            if (tree.isLeaf())
            {
                return matchLeaf(tree);
            }
            //we know that we have a list
            else
            {
                ListNode list = (ListNode) tree;

                if (list.values[0].isLeaf())
                {
                    return matchExpression((ListNode) tree);               
                }
                // if we have a list of lists we either have a body or a top level form...
                else
                {
                    foreach (Node n in list.getList())
                    {
                        expressionList.Add(match(n));
                    }
                    ParameterExpression env = Expression.Variable(typeof(Environment), "env");
                    return Expression.Block(new ParameterExpression[] { env }, expressionList);
                }
            }
        }

        // This matches an expression of some type
        static Expression matchExpression(ListNode tree)
        {
            switch (tree.values[0].getValue())
            {

                case "+":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse plus for list " + tree.ToString());
                    return Expression.Add(match(tree.values[1]), match(tree.values[2]));
                case "-":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse minus for list " + tree.ToString());
                    return Expression.Subtract(match(tree.values[1]), match(tree.values[2]));
                case "*":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse times for list " + tree.ToString());
                    return Expression.Multiply(match(tree.values[1]), match(tree.values[2]));
                case "/":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse divide for list " + tree.ToString());
                    return Expression.Divide(match(tree.values[1]), match(tree.values[2]));
                case "%":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse mod for list " + tree.ToString());
                    return Expression.Modulo(match(tree.values[1]), match(tree.values[2]));

                case "equals":
                    if (tree.values.Count != 3)
                        throw new ParsingException("failed to parse equals for list " + tree.ToString());
                    return Expression.Equal(match(tree.values[1]), match(tree.values[2]));
                case "if":
                    throw new NotImplementedException();
                case "define":
                    if (tree.values.Count != 3 || tree.values[1].isList())
                        throw new ParsingException("failed to parse define");
                    //TODO make this id the type with reflection
                    ParameterExpression var = Expression.Parameter(typeof(int), tree.values[1].getValue());
                    Expression envExpression = Expression.Variable(typeof(Environment), "env");
                    //Console.WriteLine(Expression.Constant(tree.values[1].getValue()).Type.ToString());
                    Expression call = Expression.Call(
                        envExpression,
                        typeof(Environment).GetMethod("add", new Type[] { typeof(String), typeof(int) }),
                        Expression.Constant(tree.values[1].getValue()),
                        var);
                    Expression check = Expression.Call(
                        envExpression,
                        typeof(Environment).GetMethod("lookup", new Type[] { typeof(String) }),
                        Expression.Constant("x"));
                    return Expression.Block(new[] { var }, Expression.Assign(var, match(tree.values[2])), check);
                case "lambda":
                    throw new NotImplementedException();
                case "cons":
                    throw new NotImplementedException();
                case "car":
                    throw new NotImplementedException();
                case "cdr":
                    throw new NotImplementedException();

            }
        
            throw new NotImplementedException();
        }

        static Expression matchLeaf(Node leaf)
        {
            bool matchedAtom;
            Expression e;
            e = matchAtom(leaf, out matchedAtom);
            if (matchedAtom)
            {
                return e;
            }
            else
            {
                return Expression.Block( new ParameterExpression[] { Expression.Parameter(typeof(Environment), "env")},
                            Expression.Call(
                            Expression.Variable(typeof(Environment), "env"),
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
            //TODO make this understand how scheme does litearl lists aka '(blah blag) vs 'blah
            if (value[0] == '\'')
            {
                isAtom = true;
                return Expression.Constant(value, typeof(String));
            }
            //TODO add support for void

            isAtom = false;
            return null;

        }

        static Tuple<ListNode, int> captureList(string s, int index)
        {
            List<Node> values = new List<Node>();
            while (index < s.Length)
            {
                if (s[index] == ')')
                {
                    index += 1;
                    break;
                }
                if (s[index] == '(')
                {
                    index += 1;
                    Tuple<ListNode, int> ret = captureList(s, index);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                if (Regex.IsMatch(s[index].ToString(), patternNotWhitespaceOrParen))
                {
                    Tuple<LeafNode, int> ret = captureLeaf(s, index);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                index += 1;
            }
            return Tuple.Create<ListNode, int>(new ListNode(values), index);
        }

        static Tuple<LeafNode, int> captureLeaf(String s, int index)
         {
             String atom;
             Regex r ;
             
             r = new Regex(patternMatchAtom);
             Match m = r.Match(s, index);
             atom = m.Value;
             r = new Regex(patternMatchAtomNoWhiteSpace);
             atom = r.Match(atom).Value;
             
             return Tuple.Create<LeafNode, int>(new LeafNode(atom), m.Index + m.Length);
         }

        static Expression MatchVariableDefines(String s)
        {
            return Expression.Block();
        }

        static Expression MatchFunctionDefine(String s)
        {
            return Expression.Block();
        }

        static Expression MatchExpression(String s)
        {
            return Expression.Block();
        }

        static void PrintHelp()
        {
            Console.WriteLine("if this were a real project then we would print a help document here");
        }
    }

    interface Node 
    {
        String getValue();
        bool isList();
        bool isLeaf();
        List<Node> getList();
        void print(String s);
    }

    class LeafNode : Node
    {
        private String value;
        public LeafNode(String atom)
        {
            value = atom;
        }

        public String getValue()
        {
            return value;
        }

        public List<Node> getList()
        {
            throw new Exception("Tried to get the list from an atom");
        }

        public bool isList()
        {
            return false;
        }
        public bool isLeaf()
        {
            return true;
        }
        public void print(String s)
        {
            Console.WriteLine(s +  this.ToString());
        }

        public override String ToString()
        {
            return this.value;
        }
    }

    class ListNode : Node
    {
        public List<Node> values;
        public ListNode(List<Node> _values)
        {
            values = _values;
        }

        public string getValue()
        {
            throw new NotImplementedException();
        }

        public bool isList()
        {
            return true;
        }

        public bool isLeaf()
        {
            return false;
        }

        public List<Node> getList()
        {
            return values;
        }

        public void print(string s)
        {
            foreach (Node n in values)
            {
                n.print(s + "  ");
            }
        }

        public override String ToString()
        {
            String ret = "";
            foreach (Node n in values)
            {
                ret += n.ToString() + "\n";
            }
            return ret;
        }

        public void print()
        {
            print("");
        }
    }
}
