using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace SimpleSchemeParser
{

    public class SchemeParser
    {

        //Regex patterns
        static String patternNotWhitespaceOrParen = "[^ \t$()\r\n]";
        static String patternMatchAtom = "[ \t\r\n]*[^ ()\t\r\n]+[ \t\r\n]*";
        static String patternMatchAtomNoWhiteSpace = "[^ \t()\r\n]*";

        String filename;

        public SchemeParser(String _filename)
        {
            filename = _filename;
        }

        public ListNode parseFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    return captureList(sr.ReadToEnd(), 0, 0).Item1;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error parsing file: " + e.Message); 
            }

        }

        private Tuple<ListNode, int> captureList(string s, int index, int nesting)
        {
            bool litearal = false;
            List<Node> values = new List<Node>();
            while (index < s.Length)
            {
                if (litearal)
                {
                    if (s[index] == ')')
                    {
                        litearal = false;
                        index += 1;
                        continue;
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }

                }
                if ( (index + 1 < s.Length) && s[index] == '\'' && s[index + 1] == '(')
                {
                    //we have a litearal we should not parse it
                    litearal = true;
                }
                if (s[index] == ')')
                {
                    index += 1;
                    break;
                }
                if (s[index] == '(')
                {
                    index += 1;
                    Tuple<ListNode, int> ret = captureList(s, index, nesting + 1);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                if (Regex.IsMatch(s[index].ToString(), patternNotWhitespaceOrParen))
                {
                    Tuple<LeafNode, int> ret = captureLeaf(s, index, nesting);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                    continue;
                }
                index += 1;
            }
            return Tuple.Create<ListNode, int>(new ListNode(values, nesting), index);
        }

        private Tuple<LeafNode, int> captureLeaf(String s, int index, int nesting)
        {
            String atom;
            Regex r;

            r = new Regex(patternMatchAtom);
            Match m = r.Match(s, index);
            atom = m.Value;
            r = new Regex(patternMatchAtomNoWhiteSpace);
            atom = r.Match(atom).Value;

            return Tuple.Create<LeafNode, int>(new LeafNode(atom, nesting), m.Index + m.Length);
        }

    }
}
