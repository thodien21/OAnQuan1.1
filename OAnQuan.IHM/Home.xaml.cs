using OAnQuan.Business;
using OAnQuan.DataAccess;
using System;
using System.Collections.Generic;
using System.Windows;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Logique d'interaction pour WindowMainMenu.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();

            //Display the best players
            icBestPlayerList.ItemsSource= PlayerDb.GetRankingPlayerListWithLimit(5);
            txbWelcome.Text = "Bienvenu "+ Services.Player.Pseudo + " !";

            //Display info of player
            tbiPlayer.DataContext = Services.Player;
            lblOwnRanking.Content = Services.Player.Ranking + "/" + Services.PlayerQty;
            lblIsAdmin.Content = (Services.Player.IsAdmin == 1) ? "Oui" : "Non";
            lblIsDisabled.Content = (Services.Player.IsEnabled == 1) ? "Oui" : "Non";

            //Administration
            if(Services.Player.IsAdmin != 1)
            {
                btnAdmin.Visibility = Visibility;//Hide this button since player is not admin
            }
        }
        
        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.Show();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(Services.Player.IsEnabled == 0)
            {
                MessageBox.Show("Votre compte est désactivé, vous ne pouvez plus jouer...");
            }
            else
            {
                AskPseudoPlayer2 pseudoPlayer2 = new AskPseudoPlayer2();
                pseudoPlayer2.ShowDialog();
            }
        }
        
        private void btnPlaySavedGame_Click(object sender, RoutedEventArgs e)
        {
            if (BoardDb.CheckIfBoardDbContainsPlayerId(Services.Player.PlayerId))
            {
                Services.NoveltyOfGame = NoveltyOfGame.OLD;
                Services.Player.GetSavedGameFromDb();
                PlayGame game = new PlayGame();
                game.ShowDialog();
            }
            else
                MessageBox.Show("Vous n'avez pas encore de partie sauvegardée");
        }
    }
}
