using NUnit.Framework;
using OAnQuan.Business;
using OAnQuan.DataAccess;
using System.Collections.Generic;

namespace OAnQuan.Test.DataAccess
{
    [TestFixture]
    public static class PlayerDbTest
    {
        [Test]
        public static void GetAllPlayer_Test()
        {
            List<Player> playerList = PlayerDb.GetAllPlayer();
            Assert.AreEqual(playerList.Count, 8);
            var player1 = new List<string>() { "", "" };
            var player2 = new List<string>() { "Ti", "Chuot" };
            var player3 = new List<string>() { "Suu", "Trau" };
            var player4 = new List<string>() { "Dan", "Cop" };
            var player5 = new List<string>() { "Meo", "Meo" };
            var player6 = new List<string>() { "Thin", "Rong" };
            var player7 = new List<string>() { "Ty", "Ran" };
            var player8 = new List<string>() { "Ngo", "Ngua" };

            Assert.AreEqual(ShowPlayer(playerList[0]), player1);
            Assert.AreEqual(ShowPlayer(playerList[1]), player2);
            Assert.AreEqual(ShowPlayer(playerList[2]), player3);
            Assert.AreEqual(ShowPlayer(playerList[3]), player4);
            Assert.AreEqual(ShowPlayer(playerList[4]), player5);
            Assert.AreEqual(ShowPlayer(playerList[5]), player6);
            Assert.AreEqual(ShowPlayer(playerList[6]), player7);
            Assert.AreEqual(ShowPlayer(playerList[7]), player8);
        }
        public static List<string> ShowPlayer(Player player)
        {
            return new List<string>() { player.Pseudo, player.FullName };
        }

        [Test]
        public static void RankingPlayer_Test()
        {
            List<Player> playerList = PlayerDb.GetRankingPlayerListWithLimit(3);
            Assert.AreEqual(playerList.Count, 3);

            //var player1 = new List<string>() { "", "0", "0", "0" };
            //var player2 = new List<string>() { "Ty", "3", "5", "2" };
            //var player3 = new List<string>() { "Suu", "13", "4", "4" };
            var player4 = new List<string>() { "Dan", "13", "5", "1" };
            var player5 = new List<string>() { "Meo", "14", "3", "3" };
            //var player6 = new List<string>() { "Thin", "2", "34", ""39" };
            var player7 = new List<string>() { "Ty", "17", "0", "0" };

            Assert.AreEqual(RankingPlayer(playerList[0]), player7);
            Assert.AreEqual(RankingPlayer(playerList[1]), player5);
            Assert.AreEqual(RankingPlayer(playerList[2]), player4);
            //Assert.AreEqual(RankingPlayer(playerList[3]), player3);
            //Assert.AreEqual(RankingPlayer(playerList[4]), player2);
            //Assert.AreEqual(RankingPlayer(playerList[5]), player6);
            //Assert.AreEqual(RankingPlayer(playerList[6]), player1);

        }
        public static List<string> RankingPlayer(Player player)
        {
            return new List<string>() { player.Pseudo, player.WinGameQty.ToString(), player.DrawGameQty.ToString(), player.LoseGameQty.ToString() };
        }

        [Test]
        public static void UpgradePlayerToAdmin()
        {
            //PlayerDb.UpgradePlayerToAdmin(2, 8);
            //PlayerDb.UpgradePlayerToAdmin(5, 6);
        }
    }
}
