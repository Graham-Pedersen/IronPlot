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
        List<Node> getList();
        void print(String s);
    }

    public class LeafNode : Node
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
            Console.WriteLine(s + this.ToString());
        }

        public override String ToString()
        {
            return this.value;
        }
    }

    public class ListNode : Node
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
