using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRLanguage.FileState
{
    public class plotFile
    {

        private string _code;
        private string _name;
        private List<String> MethodTips;
        private Dictionary<String, List<String>> function_vars; //name of function list of available vars.

        public plotFile()
        {
        }
        public plotFile(string name, string code)
        {
            _name = name;
            _code = code;
        }

      //
      //  public string getCode() { get _code; }
    }
}
