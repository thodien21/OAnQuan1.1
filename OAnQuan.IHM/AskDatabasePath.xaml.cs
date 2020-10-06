using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Interaction logic for AskDatabasePath.xaml
    /// </summary>
    public partial class AskDatabasePath : Window
    {
        public AskDatabasePath()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //if(txbDBPath.Text.Contains("/DatabaseOAQ.db"))
            //    Services.DbPath = "Data Source= " + txbDBPath.Text + ";Version=3;New=True;Compress=True;";
            //else
            //    Services.DbPath = "Data Source= " + txbDBPath.Text + "/DatabaseOAQ.db;Version=3;New=True;Compress=True;";
            this.Hide();
        }
    }
}
