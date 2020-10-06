using OAnQuan.Business;
using OAnQuan.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;

namespace OAnQuan
{
    public static class Services
    {
        /// <summary>
        /// Player who login/sign up
        /// </summary>
        public static Player Player { get; set; }
        public static List<Player> PlayerListWithRanking => PlayerDb.GetRankingPlayerListWithLimit(PlayerDb.CountPlayer());
        public static int PlayerQty => PlayerListWithRanking.Count;
        public static string ConnectionString => GetConnectionString();
        public static string GetConnectionString()
        {
            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var path = root + "\\OAnQuan\\DataAccess\\DatabaseOAQ.db";
            var stringConnection = "Data Source= " + path + "; Version = 3; New = True; Compress = True;";
            return stringConnection;
        }

        /// <summary>
        /// Get own player from login/sign up
        /// </summary>
        /// <param name="pseudo"></param>
        /// <param name="pass"></param>
        public static void GetPlayer(string pseudo, string pass)
        {
            Player = PlayerDb.GetPlayer(pseudo, pass);
        }

        //To know if the game is new or taken from saved game
        public static NoveltyOfGame NoveltyOfGame { get; set; }

        /// <summary>
        /// To require when player 1 plays
        /// </summary>
        public static string PseudoPlayer2 { get; set; }
    }
}
