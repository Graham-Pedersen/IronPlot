using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSchemeParser
{
    public interface Node
    {
        String getValue();
        bool isList();
        bool isLeaf();
        bool isTopLevel();
        List<Node> getList();
        void print(String s);
        int getNestingLevel();
    }

    public class LeafNode : Node
    {
        private String value;
        private int nestingLevel;
        public LeafNode(String atom, int _nestingLevel)
        {
            value = atom;
            nestingLevel = _nestingLevel;
        }

        public int getNestingLevel()
        {
            return nestingLevel;
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
        public bool isTopLevel()
        {
            return false;
        }
        public void print(String s)
        {
            Console.WriteLine(s + this.ToString());
        }

        public override String ToString()
        {
            return this.value;
        }
    }

    public class ListNode : Node
    {
        public bool isLiteral;
        public List<Node> values;
        private int nestingLevel;
        public ListNode(List<Node> _values, int _nestingLevel, bool _isLiteral)
        {
            isLiteral = _isLiteral;
            values = _values;
            nestingLevel = _nestingLevel;
        }
        public int getNestingLevel()
        {
            return nestingLevel;
        }

        public string getValue()
        {
            throw new NotSupportedException();
        }

        public bool isList()
        {
            return true;
        }

        public bool isTopLevel()
        {
            return false;
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
