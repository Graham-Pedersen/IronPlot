using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLR_Compiler;

namespace Desugar_Compiler_TestHarness
{
    class Program
    {


        private static void DeleteEmittedExe(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string InvokeAndReturnOutput(string input, string output_exe)
        {
            String dir = Directory.GetCurrentDirectory();
            dir += @"\" + output_exe;


            ProcessStartInfo Invoke = new ProcessStartInfo(dir);
            Invoke.UseShellExecute = false;
            Invoke.CreateNoWindow = false;
            Invoke.RedirectStandardError = true;
            Invoke.RedirectStandardOutput = true;
            try
            {
                Process InvokeEXE = Process.Start(Invoke);
                StreamReader sr = InvokeEXE.StandardOutput;
                StreamReader sr2 = InvokeEXE.StandardError;
                string output = sr.ReadToEnd();
                string output2 = sr2.ReadToEnd();
                sr.Close();
                sr2.Close();
                InvokeEXE.Close();
                if (output2 != string.Empty)
                {
                    DeleteEmittedExe(output_exe);
                    return output2;
                }
                DeleteEmittedExe(output_exe);
                return output;
            }
            catch (Exception e)
            {
                DeleteEmittedExe(output_exe);
                return e.Message;
            }
        }

        private static string Runcompiler(string inputfile, string output_exe)
        {
            try
            {
                DLR_Compiler.Compiler.compile(inputfile, "compile", output_exe);
                return InvokeAndReturnOutput(inputfile, output_exe);
            }
            catch (Exception e)
            {
                Console.WriteLine("!!!!!!!! FAILED TEST !!!!!!!!!!!");
                Console.WriteLine("For InputFile " + inputfile + " The compiler has output on STDERR:");
                Console.WriteLine(e.Message);
            }
            return String.Empty;
        }


       static bool RunDesugar(string inputfile, string tempfile)
        {
            //Start the exe and forward standard output/error so we can parse etc.
            //ProcessStartInfo Desugar = new ProcessStartInfo(@"C:\Users\graha_000\Programing\IronPlot\desugar.exe", String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
            ProcessStartInfo Desugar = new ProcessStartInfo(@"C:\Users\Scott\Documents\Compiler\IronPlot\desugar.exe", String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
            Desugar.UseShellExecute = false;
            Desugar.CreateNoWindow = false;
            Desugar.RedirectStandardError = true;
            Desugar.RedirectStandardOutput = true;
            try
            {
                Process DesugarEXE = Process.Start(Desugar);
                StreamReader sr = DesugarEXE.StandardOutput;
                StreamReader sr2 = DesugarEXE.StandardError;
                string output = sr.ReadToEnd();
                string output2 = sr2.ReadToEnd();
                if (output2 != String.Empty)
                {
                    Console.WriteLine("!!!!!!!! FAILED TEST !!!!!!!!!!!");
                    Console.WriteLine("For test file " + inputfile + " Desugar'r outputed this to STDERR");
                    Console.WriteLine(output2);
                    return false;
                }
                DesugarEXE.WaitForExit();
                DesugarEXE.Close();
                return true;
            }
            catch (Exception e)
            {

            }
           return false;
        }


       static void cleanuptmp(string dir)
       {
           string[] files = Directory.GetFiles(dir);
           foreach (string s in files)
           {
               if (s.Contains(".tmp"))
               {
                   System.IO.File.Delete(s);
               }
           }
       }

        static void Main(string[] args)
        {
            String output = String.Empty;
            //string[] files = Directory.GetFiles(@"C:\Users\graha_000\Programing\IronPlot\test");
            string[] files = Directory.GetFiles(@"C:\Users\Scott\Documents\Compiler\IronPlot\test");

            foreach (string s in files)
            {
                if (s.Contains(".plot") && !s.Contains(".tmp"))
                {
                    string Filename = Path.GetFileNameWithoutExtension(s);
                    Console.Write("Testing " + Filename);
                        if (RunDesugar(s, s + ".tmp"))
                        {
                            output = Runcompiler(s + ".tmp", Filename + ".exe");
                            if (output != "Passed\r\n")
                            {
                                Console.WriteLine();
                                Console.WriteLine("!!!!!!!! FAILED TEST !!!!!!!!!!!");
                                Console.WriteLine("Filename: " + Filename + ".plot failed.");
                                Console.WriteLine("Expected: " + "Passed\r\n" + "Got: " + output);
                            }
                            else
                            {
                                Console.WriteLine(" Passed");
                            }
                        }
                        //Console.ReadKey();
                    }
                }
            Console.WriteLine("Cleaning up");
            //cleanuptmp(@"C:\Users\graha_000\Programming\IronPlot\test");
            cleanuptmp(@"C:\Users\Scott\Documents\Compiler\IronPlot\test");
            Console.ReadKey();
        }
    }
}
