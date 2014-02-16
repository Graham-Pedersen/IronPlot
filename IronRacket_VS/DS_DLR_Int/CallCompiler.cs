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

        public CallCompiler(string name){
            //writer = new StreamWriter("dlr_tmp.tmp");
            D_exep = DLR_exep = string.Empty;
            CheckForCompiler();
            _name = @name;
            if (!string.IsNullOrEmpty(D_exep) && !string.IsNullOrEmpty(DLR_exep))
            {
                RunDesugar(name,name+".tmp");
                RunDLR(name+".tmp");
            }
        }

        private static void CheckForCompiler()
        {
            try
            {
                //cwd = System.IO.Directory.GetCurrentDirectory();
                D_exep = "C:\\Users\\Scotty\\Desktop\\Backup\\DS_DLR_Int";
                if(File.Exists(D_exep + "\\Desugar.exe")){
                    D_exep = D_exep+"\\Desugar.exe";
                }
                if(File.Exists(D_exep + "\\DLR.exe")){
                    DLR_exep = D_exep + "\\DLR.exe";
                }
                else{
                    throw new FileNotFoundException("Couldn't find DLR or Desugar !\n");
                }
            }
            catch(IOException e){
                throw e;
            }

        }

        private void RunDLR(string inputtemp)
        {
            ProcessStartInfo Dlr = new ProcessStartInfo(DLR_exep, inputtemp);
            Dlr.UseShellExecute = true;
            Dlr.CreateNoWindow = false;
            try
            {
                Process DlrEXE = Process.Start(Dlr);
                DlrEXE.WaitForExit();
                DlrEXE.Close();
                File.Delete(inputtemp);
            }
            catch (Exception e)
            {

            }
        }

        private void RunDesugar(string inputfile,string tempfile)
        {
            //Start the exe and forward standard output/error so we can parse etc.
            ProcessStartInfo Desugar = new ProcessStartInfo(D_exep, inputfile + " "+tempfile);
            Desugar.UseShellExecute = true;
            Desugar.CreateNoWindow = false;
            //ffmpeg.RedirectStandardError = true;
            //ffmpeg.RedirectStandardOutput = true;
            try
            {
                Process DesugarEXE = Process.Start(Desugar);
                DesugarEXE.WaitForExit();
                DesugarEXE.Close();
            }
            catch (Exception e)
            {
               
            }

        }
    }
}
