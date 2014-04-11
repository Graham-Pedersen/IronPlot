using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DS_DLR_Int
{
    public class CompileTarget : Microsoft.Build.Utilities.Task
    {

        private string SourceFile;
        private string comptype;
        public override bool Execute()
       {
            CallCompiler c = new CallCompiler(SourceFile,comptype); 
            return true;
        }
        public string SourceFiles
        {
            get { return SourceFiles; }
            set { SourceFile = value; }
        }
        public string CompiliationType
        {
            get { return comptype; }
            set { comptype = value; }
        }

    }
}
