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
                    List_Node topLevelForms = captureList(sr.ReadToEnd(), 0).Item1;
                    topLevelForms.print();
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("File does not exist");
            }
            Console.ReadKey();
        }

        static Expression Emit_DLR_Tree(List<String> topLevels)
        {
            List<Expression> topLevelExpressions = new List<Expression>();
            return Expression.Block();   
        }

        static Tuple<List_Node, int> captureList(string s, int index)
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
                    Tuple<List_Node, int> ret = captureList(s, index);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                if (Regex.IsMatch(s[index].ToString(), patternNotWhitespaceOrParen))
                {
                    Tuple<Atom_Node, int> ret = captureAtom(s, index);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                index += 1;
            }
            return Tuple.Create<List_Node, int>(new List_Node(values), index);
        }

         static Tuple<Atom_Node, int> captureAtom(String s, int index)
         {
             String atom;
             Regex r ;
             
             r = new Regex(patternMatchAtom);
             Match m = r.Match(s, index);
             atom = m.Value;
             r = new Regex(patternMatchAtomNoWhiteSpace);
             atom = r.Match(atom).Value;
             
             return Tuple.Create<Atom_Node, int>(new Atom_Node(atom), m.Index + m.Length);
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
        String getAtom();
        bool isList();
        bool isAtom();
        List<Node> getList();
        void print(String s);
    }

    class Atom_Node : Node
    {
        private String value;
        public Atom_Node(String atom)
        {
            value = atom;
        }

        public String getAtom()
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
        public bool isAtom()
        {
            return true;
        }
        public void print(String s)
        {
            Console.WriteLine(s + this.value);
        }
    }

    class List_Node : Node
    {
        List<Node> values;
        public List_Node(List<Node> _values)
        {
            values = _values;
        }

        public string getAtom()
        {
            throw new NotImplementedException();
        }

        public bool isList()
        {
            return true;
        }

        public bool isAtom()
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
