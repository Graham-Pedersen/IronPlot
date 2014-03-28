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

        public MyControl()
        {
            InitializeComponent();
            //TextChangedEventHandler h = TextChanged;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "IronRacket REPL");

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            if(sender.GetType() == typeof(System.Windows.Controls.TextBox)){
                TextBox tb = (TextBox)sender;

            }
            TextChangedEventHandler h = TextChanged;
            if (h != null)
            {
                h(this, args);
            }
        }

    }
}