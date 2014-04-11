
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace None.IronRacket_ReplWindow
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {

        public event TextChangedEventHandler TextChanged;
        TextPointer tp;
        Paragraph p;
        RichTextBox rtb;
        Evaluator.Evaluator ev; 

        public MyControl()
        {
            InitializeComponent();
            ev = new Evaluator.Evaluator();
            RichTextBox myRichTextBox = new RichTextBox();

            // Create a FlowDocument to contain content for the RichTextBox.
            FlowDocument myFlowDoc = new FlowDocument();

            // Add initial content to the RichTextBox.
            Repl.Document = myFlowDoc;
            Repl.AppendText("> ");
        }


        private bool matchParens(string ss)
        {
            bool flag = false;
            int left = 0;
            int i;
            for (i = 0; i < ss.Length; i++)
            {
                switch (ss[i])
                {
                    case '(':
                        flag = true;
                        left++;
                        break;
                    case ')':
                        flag = true;
                        left--;
                        break;

                }
            }
            if (left == 0 && flag)
            {
                return true;
            }
            return false;
        }

        private bool handleEnter()
        {

            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                Repl.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                Repl.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            String s = textRange.Text;
            int last_i_of = s.LastIndexOf(">");
            if (last_i_of < 0)
            {
                Repl.AppendText("Error, this repl window sucks real bad, there _MUST_ be a >. Blame crappy C# WPF code\r\n");
                Repl.AppendText("> " + s);
                return true;
            }

            String ss = s.Substring(last_i_of+1);
            if (matchParens(ss))
            {
                Repl.AppendText("\n");
                foreach (string x in ev.evaluate(ss))
                {
                    Repl.AppendText(x+"\n");
                }
                Repl.AppendText("> ");
                Repl.CaretPosition = Repl.Document.ContentEnd;
                return true;
            }

            Repl.AppendText("\nParen Mismatch detected\n");
            Repl.AppendText("> " + ss.TrimEnd());
            Repl.CaretPosition = Repl.Document.ContentEnd;
            return true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Enter:
                    e.Handled = handleEnter();
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "IronRacket REPL");

        }


        private void Repl_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

     /*   private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            if(sender.GetType() == typeof(System.Windows.Controls.TextBox)){
                TextBox tb = (TextBox)sender;
                int temp = tb.Text.LastIndexOf(";");
                int temp2 = tb.Text.Length;
                if (tb.Text.LastIndexOf(";") == tb.Text.Length-1)
                {
                    MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}", this.ToString()),
                            "IronRacket REPL");
                }
            }
        }*/

    }
}