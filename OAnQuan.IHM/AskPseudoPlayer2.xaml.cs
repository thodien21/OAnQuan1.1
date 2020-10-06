using OAnQuan.Business;
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
    /// Logique d'interaction pour AskPseudoPlayer2.xaml
    /// </summary>
    public partial class AskPseudoPlayer2 : Window
    {
        public AskPseudoPlayer2()
        {
            InitializeComponent();
            lblPseudo.Content = "Bienvenu " + Services.Player.Pseudo + "\nSaisissez le pseudo de 2ème joueur :";
            txbPseudo.Focus();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (txbPseudo.Text.Trim( ) == "")
                MessageBox.Show("Le pseudo ne doit pas être vide");
            Services.PseudoPlayer2 = txbPseudo.Text;

            this.Hide();
            Services.NoveltyOfGame = NoveltyOfGame.NEW;
            PlayGame game = new PlayGame();
            game.ShowDialog();
        }
    }
}
