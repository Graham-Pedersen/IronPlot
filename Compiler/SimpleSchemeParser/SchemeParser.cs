﻿using System;
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
        //static String patternMatchAtom = "[ \t\r\n]*[^ ()\t\r\n]+[ \t\r\n]*";
        static String patternMatchAtom = "[ \t\r\n]*[^ ()\t\r\n]+[ \t\r\n]*";
        static String patternMatchAtomNoWhiteSpace = "[^ \t()\r\n]*";
        static String patternMatchWhitespace = "[\\s]";
        static String patternMatchString = "[$\"]";

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

        private Tuple<LeafNode, int> captureLiteral(string s, int index, int nesting)
        {
            String atom;
            Regex r;
            r = new Regex(patternMatchAtom);
            if (r.IsMatch(s, index))
            {
                Match m = r.Match(s, index);
                atom = m.Value;
                r = new Regex(patternMatchAtomNoWhiteSpace);
                atom = r.Match(atom).Value;
                return Tuple.Create<LeafNode, int>(new LeafNode(atom, nesting), m.Index + m.Length);
            }
            else
            {
                throw new Exception("Error parsing literal \n"); // I dont believe it will ever get here
            }
        //    return null;
        }

        private Tuple<ListNode, int> captureLiteralList(string s, int index, int nesting)
        {
            List<Node> values = new List<Node>();
            while (index < s.Length)
            {
                if (s[index] == '(')
                {
                    index += 1;
                    Tuple<ListNode, int> ret = captureLiteralList(s, index, nesting);
                    index = ret.Item2;
                    ret.Item1.isLiteral = true;
                    values.Add(ret.Item1);
                }
                if (s[index] == ')')
                {
                    index += 1;
                    break;
                }
                if (Regex.IsMatch(s[index].ToString(), patternNotWhitespaceOrParen))
                {
                    Tuple<LeafNode, int> ret = captureLiteral(s, index, nesting);
                    index = ret.Item2;
                    ret.Item1.isLiteral = true;
                    values.Add(ret.Item1);
                    continue;
                }
                index += 1;
            }
            return Tuple.Create<ListNode, int>(new ListNode(values, nesting, false), index);
         //   return Tuple.Create<ListNode, int>(values, index);
        }

        private Tuple<ListNode, int> captureList(string s, int index, int nesting)
        {
            List<Node> values = new List<Node>();
            while (index < s.Length)
            {
                if (s[index] == '\'')
                {
                    index += 1;
                    if ((index < s.Length) && s[index] == '(') // literal list case
                    {
                        index += 1;
                        Tuple<ListNode, int> ret = captureLiteralList(s, index, nesting + 1);
                        index = ret.Item2;
                        ret.Item1.isLiteral = true;
                        values.Add(ret.Item1);
                        continue;
                    }
                    else
                    {
                        Tuple<LeafNode, int> ret = captureLiteral(s, index, nesting);
                        index = ret.Item2;
                        ret.Item1.isLiteral = true;
                        values.Add(ret.Item1);
                        continue;
                    }
                }
                if ((index + 6 < s.Length) && s[index] == '(' && s[index + 1] == 'q'
                        && s[index + 2] == 'u' && s[index + 3] == 'o' && s[index + 4] == 't'
                        && s[index + 5] == 'e' && Regex.IsMatch(s[index + 6].ToString(), patternMatchWhitespace)) // need to match all whitespace
                {
                    // must have special check for another opening paren, signaling a literal list
                    index += 7;
                    if ((index + 1 < s.Length) && s[index] == '(')
                    {
                        index += 1;
                        Tuple<ListNode, int> ret = captureLiteralList(s, index, nesting + 1);
                        index = ret.Item2;
                        ret.Item1.isLiteral = true;
                        values.Add(ret.Item1);
                    }
                    else
                    {
                        Tuple<LeafNode, int> ret = captureLiteral(s, index, nesting);
                        index = ret.Item2;
                        ret.Item1.isLiteral = true;
                        values.Add(ret.Item1);
                    }
                    // need to match whitespace at end, and closing paren of quote
                    while (index < s.Length && s[index] != ')')
                    {
                        if (Regex.IsMatch(s[index].ToString(), patternMatchWhitespace))
                        {
                            // check for things that are not whitespace, because if anything but whitespace this is syntax error
                            index += 1;
                        }
                        else
                            throw new Exception("Error parsing literal list in quote form \n");

                    }
                    index += 1;
                    continue;
                }
                if (s[index] == '\"')
                {   
                    int start = index;
                    index += 1;
                    while (s[index] != '\"')
                    {
                        index += 1;
                    }
                    values.Add(new LeafNode(s.Substring(start, (index - start)), nesting));
                    index += 1;
                    continue;
                }
                if (s[index] == ')')
                {
                    index += 1;
                    //Console.Write(index);
                    break;
                }
                if (s[index] == '(')
                {
                    index += 1;
                    Tuple<ListNode, int> ret = captureList(s, index, nesting + 1);
                    index = ret.Item2;
                    values.Add(ret.Item1);
                  //  index += 1;
                   // Console.Write(s[index]);
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

            return Tuple.Create<ListNode, int>(new ListNode(values, nesting, false), index);
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

        private Tuple<LeafNode, int> captureString(String s, int index, int nesting)
        {
            String atom;
            Regex r;

            r = new Regex(patternMatchString);
            Match m = r.Match(s, index);
            atom = m.Value;
          //  r = new Regex(patternMatchAtomNoWhiteSpace);
          //  atom = r.Match(atom).Value;

            return Tuple.Create<LeafNode, int>(new LeafNode(atom, nesting), m.Index + m.Length);
        }

    }
}