using OAnQuan.Business;
using OAnQuan.DataAccess;
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
    /// Interaction logic for PlayerInfo.xaml
    /// </summary>
    public partial class PlayerInfo : Window
    {
        public static DataGrid dataGrid;
        public PlayerInfo()
        {
            InitializeComponent();
            dgrListPlayer.ItemsSource = Services.PlayerListWithRanking;
            dataGrid = dgrListPlayer;
        }
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            Player thisPlayer = dgrListPlayer.SelectedItem as Player;
            
            UpdateInfoPlayer up = new UpdateInfoPlayer(thisPlayer);
            up.ShowDialog();
        }
    }
}
