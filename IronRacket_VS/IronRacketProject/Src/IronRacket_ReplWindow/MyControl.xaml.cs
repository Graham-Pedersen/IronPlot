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

        public MyControl()
        {
            InitializeComponent();
            TextChangedEventHandler h = TextChanged;
            Repl.AppendText("> ");
            Repl.CaretPosition = Repl.CaretPosition.Paragraph.ContentEnd;
            tp = Repl.CaretPosition;
        }


        private void handleDelete(){
            
        }

        private void handleEnter()
        {

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Back:
                    handleDelete();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    handleEnter();
                    e.Handled = true;
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