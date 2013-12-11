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
        static String patternWhitespace = "[ \t\r\n]";
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
                    Emit_DLR_Tree(topLevelForms);
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("File does not exist");
            }
            Console.ReadKey();
        }

        static Expression Emit_DLR_Tree(ListNode topLevels)
        {
            List<Expression> topLevelExpressions = new List<Expression>();
            return Expression.Block();   
        }

        static Expression match(Node tree)
        {
            if (tree.isLeaf())
            {
                bool matchedAtom; 
                return matchAtom(tree, out matchedAtom);
            }
            return Expression.Block();
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
                    Tuple<LeafNode, int> ret = captureAtom(s, index);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                index += 1;
            }
            return Tuple.Create<ListNode, int>(new ListNode(values), index);
        }

        static Tuple<LeafNode, int> captureAtom(String s, int index)
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
            Console.WriteLine(s + this.value);
        }
    }

    class ListNode : Node
    {
        List<Node> values;
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

        public void print()
        {
            print("");
        }
    }
}
