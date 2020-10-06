using OAnQuan.Business;
using OAnQuan.DataAccess;
using OAnQuan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Logique d'interaction pour WindowSignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
            txbPeuso.Focus();
        }

        private void BtnCreatAccount_Click(object sender, RoutedEventArgs e)
        {
            //Verify if this login already exists in database
            List<Player> allPlayer = PlayerDb.GetAllPlayer();
            var player1 = allPlayer.FirstOrDefault(s => s.Pseudo == txbPeuso.Text);

            if (player1 != null)
            {
                MessageBox.Show("Ce pseudo existe déjà, choissez un autre :");
                this.Hide();
                SignUp signUp = new SignUp();
                signUp.ShowDialog();
            }
            else
            {
                if(txbPasswordConfirmed.Password == txbPassword.Password)
                {
                    this.Hide();
                    PlayerDb.InsertPlayer(txbPeuso.Text, txbPassword.Password, txbFullName.Text);//Insert player to database
                    Services.GetPlayer(txbPeuso.Text, txbPassword.Password);//Immediately get player

                    Home click = new Home();
                    click.ShowDialog();
                }
                else MessageBox.Show("Le mot de passe confirmé n'est pas correct");
            }
        }
    }
}
