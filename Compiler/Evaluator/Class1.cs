using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Evaluator
{
    Int64 curr_addr = 0;
    public class EvaluatorException : Exception
    {
        public EvaluatorException()
        {
        }

        public EvaluatorException(String message)
            : base(message)
        { }

        public EvaluatorException(String message, Exception inner)
            : base(message, inner)
        { }
    }

    public class Evaluator
    {
        // grammar
        //  expr :: number |
        //          (+ expr expr)
        //          (- expr expr)
        //          (* expr expr)
        //          (< expr expr)
        //          (> expr expr)
        //          (<= expr expr)
        //          (>= expr expr)
        //      need to add division
        //      don't want to start with environment

        public Evaluator()
        {
            // doesn't need to initialize anything?
        }

        // external facing function
        public string evaluate(string input)
        {
            Expr expression = parse(input);
            dynamic result = eval(expression);
            return result.ToString();
        }

        // currently only parses math
        private Expr parse(string input)
        {
            string new_input = input.Trim();
            // need check for terminal, aka number or symbol
            String[] temp;
            if (new_input.Length == 1)
            {
                long res;
                if (Int64.TryParse(new_input, out res))
                    return new Expr("number", res);
                else
                    throw new EvaluatorException("currently unimplemented");
            }
            if (new_input.StartsWith("("))
            {
                temp = new_input.Split(new Char[] { '(', ')' });//, '(', ')'});
                // weird that temp[0] is empty
                // and temp[1] is where first thing goes, guess because of parens?
                return new Expr(temp[1].Trim(), parse(temp[2]), parse(temp[3]));
            }
            temp = new_input.Split(new Char[] { ' ', '\t'});
            return new Expr(temp[0].Trim(), parse(temp[1]), parse(temp[2]));

        }

        private string peek(string input)
        {
            return "";
        }
    }
}
