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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            String path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).Replace(":\\", ":/");
            //Services.DbPath = "Data Source= " + path + "/OAnQuan/OAnQuan/DatabaseOAQ.db;Version=3;New=True;Compress=True;";
            
            //When database path doesn't work:
            //AskDatabasePath pathDB = new AskDatabasePath();
            //pathDB.ShowDialog();
        }

        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SignUp signUp = new SignUp();
            signUp.ShowDialog();
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SignIn signIn = new SignIn();
            signIn.ShowDialog();
        }
    }
}
