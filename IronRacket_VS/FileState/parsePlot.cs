using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Text.RegularExpressions;

namespace IRLanguage.FileState
{
    public static class parsePlot
    {
        public static void GetMethods(string text, ref List<Completion> completions){
            text = remove_comments(text);
            text = remove_linebreaks(text);
            string function;
            int index = 0;
            int debug_Length = text.Length;
            bool already_found = false;

            while (index < text.Length)
            {
                already_found = false;
                if ((function = pull_top_level_function(text, ref index)) != null)
                {

                    //change 3/28/ lol object comparason without a compartor WHOOPS
                    foreach (Completion e in completions)
                    {
                        if (e.DisplayText == function)
                        {
                            already_found = true;
                            break;
                        }
                    }
                    if (!already_found)
                    {
                        Completion c = new Completion(function);
                        c.Description = "User defined Function";
                        //c.IconSource = Microsoft.VisualStudio.Language.Intellisense.IGlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic)
                        completions.Add(c);
                        already_found = false;
                    }
                }
            }
        }

        private static string remove_comments(string text)
        {
            string[] tokens = text.Split('\n');
            string return_string="";
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].IndexOf(";") != -1)
                {
                    tokens[i] = tokens[i].Substring(0, tokens[i].IndexOf(";"));
                }
                return_string += tokens[i];
            }
            return return_string;
        }

        private static string remove_linebreaks(string text)
        {
            return Regex.Replace(text, "\r\n","");
        }
        private static string remove_whitespace(string text)
        {
            return Regex.Replace(text, @"\s*","");
        }

        private static int getEndOfFunction(string text)
        {
            int left = 0;
            int start_index = 0;
            while (text[start_index] != '(' && start_index + 1 < text.Length && text[start_index + 1] != 'd')
            {
                start_index++;
            }
            if (start_index == text.Length) return start_index;
            
            while (start_index < text.Length)
            {
                if (text[start_index] == '(')
                {
                    left++;
                }
                else if (text[start_index] == ')')
                {
                    left--;
                }
                if (left == 0)
                {
                    break;
                }
                start_index++;
            }
          //  char c = text[start_index];
          //  char c1 = text[start_index - 1];
          //  char c2 = text[start_index - 2];
          //  char c3 = text[start_index - 3];
            return start_index;
        }

        //MUST be at a starting (
        private static string pull_top_level_function(string text, ref int start_index)
        {
            text = text.Substring(start_index);
            string[] tokens = text.Split(')');
            string TL_Function="";
            bool found = false;
            int i = 0;
            for (; i < tokens.Length; i++)
            {

                //TODO: we can probably do a indexof on the string to update the start_index before passing it to findEOFunction
                //which will fix duplicates


                //"normal define" (define (function_name __VARGS__
                if (Regex.IsMatch(tokens[i], @"\s*\(\s*define\s*\([_A-Za-z0-9]+\s*([_A-Za-z0-9]+\s*)*"))
                {
                    TL_Function = Regex.Match(tokens[i].Trim(), @"\s+\([_A-Za-z0-9]+").Value;
                    found = true;
                    break;
                }
                else if (Regex.IsMatch(tokens[i], @"\s*\(\s*define\s+[_A-Za-z0-9]+\s*\(\s*lambda"))
                {
                    TL_Function = Regex.Match(tokens[i].Trim(), @"\s+[_A-Za-z0-9]+").Value;
                    TL_Function = "(" + TL_Function.Trim();
                    found = true;
                    break;
                }
            }
            if (found)
            {
                if (text.IndexOf(tokens[i]) != -1)
                {
                    start_index += getEndOfFunction(text);
                    return TL_Function.Trim();
                }
                start_index += getEndOfFunction(text);
                return TL_Function.Trim();

            }
            start_index += getEndOfFunction(text);
            return null;

        }
    }
}
