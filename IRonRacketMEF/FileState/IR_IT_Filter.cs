using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace IRLanguage.FileState
{
    public static class IntelliSense_filterer
    {
       public static void Filter_list(List<Completion> completions, string line){
            //base case is empty or space which implies a new line which we should display the full list (which has already been populated)
            //So just return
            if (line == string.Empty || line == " " || line == "(") return;

            //next line is (char we need to start filtering -- 

            //List<Completion> full_list = IRLanguage.FileState.Source.getFunctionsList();
            List<Completion> returnlist = new List<Completion>();
            for (int i = 0; i < completions.Count; i++)
            {
                if (completions.ElementAt(i).InsertionText.StartsWith(line))
                {
                    returnlist.Add(completions.ElementAt(i));
                }
            }
            completions = returnlist;


        }

    }
}
