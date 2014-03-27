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
        private string _name;
        private static string D_exep;
        private static string DLR_exep;


        private bool ServerExample(string path){
            if (File.Exists(path)){
                Runcompiler(path);
                System.IO.File.Move(path.Substring(0, path.Length - 11) + "Foo.exe", path.Substring(0, path.Length - 11)+"Server.exe");
                Runcompiler(path.Substring(0, path.Length - 11)+"Client.plot");
                System.IO.File.Move(path.Substring(0, path.Length - 11) + "Foo.exe", path.Substring(0, path.Length - 11) + "Client.exe");
                return true;
            }
            return false;
        }

        public CallCompiler(string name){
            //writer = new StreamWriter("dlr_tmp.tmp");
            //D_exep = DLR_exep = string.Empty;
            //CheckForCompiler();
          //  if (!string.IsNullOrEmpty(D_exep))
         //   {
                //RunDesugar(name,name+".tmp");
                if(name.Contains("Server")){
                    ServerExample(name);
                }
                else{
                    Runcompiler(name);
                }

                System.IO.File.Copy(@"C:\Users\graha_000\Programing\IronPlot\Compiler\Compiler_Lib\bin\Debug\Compiler_Lib.dll", name.Substring(0, name.Length - 11) + "Compiler_Lib.dll");
              //  RunDLR(name+".tmp");
            }
          //  if (File.Exists(@"C:\Users\Scott\Desktop\Foo.exe"))
           // {
           // }
        //}

       /* private static InvokeBuiltExe(){
      //      ProcessStartInfo Desugar = new ProcessStartInfo(@"C:\Users\Scott\Desktop\Foo.exe",  String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
            Desugar.UseShellExecute = false;
            Desugar.CreateNoWindow = false;
            Desugar.RedirectStandardError = true;
            Desugar.RedirectStandardOutput = true;
        }*/

        private static void RunDLR(string file)
        {

            
            //DLR_Compiler.DLR_Compiler.compile(file);
        }

        private static void CheckForCompiler()
        {
            try
            {
                //cwd = System.IO.Directory.GetCurrentDirectory();
                D_exep = @"C:\Users\graha_000\Programing\IronPlot";
                if (File.Exists(D_exep + @"\desugar.exe"))
                {
                    D_exep = D_exep + @"\desugar.exe";
                }

            }
            catch (IOException e)
            {
            }
        }


        private static void Runcompiler(string inputfile)
        {
            ProcessStartInfo comp = new ProcessStartInfo(@"C:\Users\graha_000\Programing\IronPlot\Compiler\DLR_Compiler\bin\Debug\DLR_Compiler.exe", String.Format("\"{0}\" {1}", inputfile, "compile"));
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
