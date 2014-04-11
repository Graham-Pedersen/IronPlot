using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desugar_Compiler_TestHarness
{
    class Program
    {


        private static string Runcompiler(string inputfile, string output_exe)
        {
            ProcessStartInfo comp = new ProcessStartInfo(@"C:\Users\graha_000\Programing\IronPlot\Compiler\DLR_Compiler\bin\Debug\DLR_Compiler.exe", String.Format("\"{0}\" {1} {2}", inputfile, "run", output_exe));
            comp.UseShellExecute = false;
            comp.CreateNoWindow = true;
            comp.RedirectStandardError = true;
            comp.RedirectStandardOutput = true;
            try
            {
                Process Compile = Process.Start(comp);
                StreamReader sr = Compile.StandardOutput;
                StreamReader sr2 = Compile.StandardError;
                    string output = sr.ReadToEnd();
                string output2 = sr2.ReadToEnd();
                if (output2 != String.Empty)
                {
                    Console.WriteLine("!!!!!!!! FAILED TEST !!!!!!!!!!!");
                    Console.WriteLine("For InputFile " + inputfile + " The compiler has output on STDERR:");
                    Console.WriteLine(output2);
                    
                }
                Compile.WaitForExit();
                Compile.Close();
                return output;
            }
            catch (Exception e)
            {

            }
            return String.Empty;
        }


        static Dictionary<String, String> createResults()
        {
            Dictionary<String, String> IO = new Dictionary<String, String>();
            IO.Add("12", "1\r\n5\r\n7\r\nend of list\r\n\r\n");
            IO.Add("ApplyPlus", "13\r\n\r\n");
            IO.Add("lief", "5\r\n\r\n");
            IO.Add("5", "10\r\n\r\n");
            IO.Add("While", "10\r\n\r\n");
            IO.Add("1", "5\r\n\r\n");
            IO.Add("2", "6\r\n\r\n");
            IO.Add("3", "5\r\n\r\n");
            IO.Add("4", "True\r\n\r\n");
            IO.Add("6", "3\r\n1\r\n(foo bar 3 3 a b)\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\nf: ~s ~n\r\n2432902008176640000\r\n\r\n");
            IO.Add("7", "120\r\n40320\r\n120\r\n\r\n");
            IO.Add("10", "1\r\n2\r\n3\r\n4\r\n\r\n\r\n");
            IO.Add("11", "12\r\n\r\n");
            IO.Add("13", "3\r\n\r\n");
            IO.Add("14", "9\r\n\r\n");
            IO.Add("15", "1\r\n\r\n");
            IO.Add("16", "10\r\n\r\n");
            IO.Add("div_test1", "0.125\r\n0.5\r\n.0.125r\n\r\n");
            IO.Add("fact", "120\r\n\r\n");
            IO.Add("fizzbuz", "1\r\n2\r\nbuzz\r\n4\r\nfizz\r\nbuzz\r\n7\r\n8\r\nbuzz\r\nfizz\r\n11\r\nbuzz\r\n13\r\n14\r\nfizzbuzz\r\n16\r\n17\r\nbuzz\r\n19\r\nfizz\r\nbuzz\r\n22\r\n23\r\nbuzz\r\nfizz\r\n26\r\nbuzz\r\n28\r\n29\r\nfizzbuzz\r\n31\r\n32\r\nbuzz\r\n34\r\nfizz\r\nbuzz\r\n37\r\n38\r\nbuzz\r\nfizz\r\n41\r\nbuzz\r\n43\r\n44\r\nfizzbuzz\r\n46\r\n47\r\nbuzz\r\n49\r\nfizz\r\nbuzz\r\n52\r\n53\r\nbuzz\r\nfizz\r\n56\r\nbuzz\r\n58\r\n59\r\nfizzbuzz\r\n61\r\n62\r\nbuzz\r\n64\r\nfizz\r\nbuzz\r\n67\r\n68\r\nbuzz\r\nfizz\r\n71\r\nbuzz\r\n73\r\n74\r\nfizzbuzz\r\n76\r\n77\r\nbuzz\r\n79\r\nfizz\r\nbuzz\r\n82\r\n83\r\nbuzz\r\nfizz\r\n86\r\nbuzz\r\n88\r\n89\r\nfizzbuzz\r\n91\r\n92\r\nbuzz\r\n94\r\nfizz\r\nbuzz\r\n97\r\n98\r\nbuzz\r\ndone\r\n#<void>\r\n\r\n");
            IO.Add("func_call", "5\r\n\r\n");
            IO.Add("graham", "10\r\n\r\n");
            IO.Add("less_greater_than", "True\r\nFalse\r\nTrue\r\nFalse\r\n\r\n");
            IO.Add("minus_test1", "-5\r\n0\r\n-5\r\n\r\n");
            IO.Add("mult_test1", "24\r\n24\r\n24\r\n\r\n");
            IO.Add("plust_test1", "10\r\n22\r\n16\r\n\r\n");
            IO.Add("quote", "sarah\r\n(1 2 3 4 5)\r\n(1 2 3)\r\ns\r\n5\r\n6\r\n\r\n");
            IO.Add("string_whistespace", " sarah\r\n\r\n");
            IO.Add("when", "true\r\nand\r\nfalse\r\n\r\n");
            IO.Add("if_tests", "hi there\r\nshould print2\r\n\r\n");
            IO.Add("unless", "should print\r\n\r\n");





            return IO;
        }

       static bool RunDesugar(string inputfile, string tempfile)
        {
            //Start the exe and forward standard output/error so we can parse etc.
            ProcessStartInfo Desugar = new ProcessStartInfo(@"C:\Users\graha_000\Programing\IronPlot\desugar.exe", String.Format("\"{0}\" \"{1}\"", inputfile, tempfile));
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
            Dictionary<String, String> IO = createResults();
            string[] files = Directory.GetFiles(@"C:\Users\graha_000\Programing\IronPlot\test");

            foreach (string s in files)
            {
                if (s.Contains(".plot") && !s.Contains(".tmp"))
                {
                    string Filename = Path.GetFileNameWithoutExtension(s);
                    Console.WriteLine("Testing " + Filename);
                        if (RunDesugar(s, s + ".tmp"))
                        {
                            output = Runcompiler(s + ".tmp", Filename + ".exe");
                            if (output != "Passed\r\n")
                            {
                                Console.WriteLine("!!!!!!!! FAILED TEST !!!!!!!!!!!");
                                Console.WriteLine("Filename: " + Filename + ".plot failed.");
                                Console.WriteLine("Expected: " + "Passed\r\n" + "Got: " + output);
                            }
                            else
                            {
                                Console.WriteLine("Filename: " + Filename + ".plot Passed");
                            }
                        }
                        //Console.ReadKey();
                    }
                }
            Console.WriteLine("Cleaning up");
            cleanuptmp(@"C:\Users\graha_000\Programming\IronPlot\test");
            Console.ReadKey();
        }
    }
}
