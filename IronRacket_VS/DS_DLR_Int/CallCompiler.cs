using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_DLR_Int
{
    public class CallCompiler
    {

        public bool failed;
        public string output;
        public string filename;

        private class Result{
           public bool failed;
           public string output;
        }

        private static string comptype;
        public CallCompiler(string name, string type){

            failed = false;
            output = string.Empty;
            filename = string.Empty;
            comptype = type;
            string directory = Path.GetDirectoryName(name);
                string[] files = Directory.GetFiles(directory);

                foreach (string s in files)
                {
                    if (s.Contains(".plot") && !s.Contains(".tmp"))
                    {
                        string Filename = Path.GetFileNameWithoutExtension(s);
                        Result R = RunDesugar(s, s + ".tmp");
                        if (R.failed)
                        {
                            failed = true;
                            output = R.output;
                            filename = Filename;
                            break;
                        }
                        Runcompiler(s + ".tmp", Filename);
                    }
                }

                if (File.Exists(directory + @"\Compiler_lib.dll"))
                {
                    File.Delete(directory + @"\Compiler_Lib.dll");
                }
                System.IO.File.Copy(@"C:\Users\Scott\Documents\Compiler\IronPlot\Compiler\Compiler_Lib\bin\Debug\Compiler_Lib.dll", directory + @"\Compiler_Lib.dll");

            }
       

        private static void Runcompiler(string inputfile, string output_name)
        {
            DLR_Compiler.Compiler.compile(inputfile, comptype, output_name);
            /*
             ProcessStartInfo comp = new ProcessStartInfo(@"C:\Users\Scott\Documents\Compiler\IronPlot\Compiler\DLR_Compiler\bin\Debug\DLR_Compiler.exe", String.Format("\"{0}\" {1} {2}", inputfile, comptype, output_name));
            comp.UseShellExecute = true;
            comp.CreateNoWindow = false;
            try
            {
                Process Compile = Process.Start(comp);
                Compile.WaitForExit();
                Compile.Close();
            }
            catch (Exception e)
            {
                ; //TODO catch error
            }
             */

        }

        private static Result RunDesugar(string inputfile,string tempfile)
        {
            //Start the exe and forward standard output/error so we can parse etc.
            ProcessStartInfo Desugar = new ProcessStartInfo(@"C:\Users\Scott\Documents\Compiler\IronPlot\desugar.exe", String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
            Desugar.UseShellExecute = false;
            Desugar.CreateNoWindow = false;
            Desugar.RedirectStandardError = true;
            Desugar.RedirectStandardOutput = true;
            string output = string.Empty;
            string output2 = string.Empty;
            Result R = new Result();

            try
            {
                Process DesugarEXE = Process.Start(Desugar);
                StreamReader sr = DesugarEXE.StandardOutput;
                StreamReader sr2 = DesugarEXE.StandardError;
                output = sr.ReadToEnd();
                output2 = sr2.ReadToEnd();
                DesugarEXE.WaitForExit();
                DesugarEXE.Close();
            }
            catch (Exception e)
            {
                output2 = e.Message;
            }
            finally
            {

                if (output2 != string.Empty)
                {
                    R.failed = true;
                    R.output = output2;

                }
                else
                {
                    R.failed = false;
                    R.output = string.Empty;
                }
            }
            return R;

        }
    }
}
