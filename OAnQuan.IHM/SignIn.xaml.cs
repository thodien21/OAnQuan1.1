using OAnQuan.DataAccess;
using System.Windows;
using OAnQuan.Business;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Logique d'interaction pour SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();
            txbPeuso.Focus();
        }
        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //Verify if this pseudo already exists
            if (PlayerDb.GetPlayer(txbPeuso.Text, txbPassword.Password) == null)
            {
                MessageBox.Show("Pseudo ou Mot de passe sont incorrect. Réessayez !");
                this.Hide();
                SignIn signIn = new SignIn();
                signIn.ShowDialog();
            }
            else
            {
                Services.GetPlayer(txbPeuso.Text, txbPassword.Password);//Immediately get player 
                Home click = new Home();
                MessageBox.Show(click, "Login réussi !");
                this.Hide();
                click.ShowDialog();
            }
        }
    }
}
