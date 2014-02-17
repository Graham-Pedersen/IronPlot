using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLR_Compiler;

namespace DS_DLR_Int
{
    public class CallCompiler
    {
        private string _name;
        private static string D_exep;
        private static string DLR_exep;

        public CallCompiler(string name){
            //writer = new StreamWriter("dlr_tmp.tmp");
            D_exep = DLR_exep = string.Empty;
            CheckForCompiler();
            _name = @name;
            if (!string.IsNullOrEmpty(D_exep))
            {
                RunDesugar(name,name+".tmp");
                RunDLR(name+".tmp");
            }
        }

        private static void RunDLR(string file)
        {
            DLR_Compiler.DLR_Compiler.compile(file);
        }

        private static void CheckForCompiler()
        {
            try
            {
                //cwd = System.IO.Directory.GetCurrentDirectory();
                D_exep = @"C:\Users\Scott\Desktop";
                if (File.Exists(D_exep + @"\desugar.exe"))
                {
                    D_exep = D_exep + @"\desugar.exe";
                }

            }
            catch (IOException e)
            {
            }
        } 

        private void RunDesugar(string inputfile,string tempfile)
        {
            //Start the exe and forward standard output/error so we can parse etc.
            ProcessStartInfo Desugar = new ProcessStartInfo(D_exep,  String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
            Desugar.UseShellExecute = false;
            Desugar.CreateNoWindow = false;
            Desugar.RedirectStandardError = true;
            Desugar.RedirectStandardOutput = true;
            
          
            try
            {
                Process DesugarEXE = Process.Start(Desugar);
                StreamReader sr = DesugarEXE.StandardOutput;
                StreamReader sr2 = DesugarEXE.StandardError;
                string output=sr.ReadToEnd();
                string output2 = sr2.ReadToEnd();
                DesugarEXE.WaitForExit();
                DesugarEXE.Close();
            }
            catch (Exception e)
            {
               
            }

        }
    }
}
