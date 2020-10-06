using OAnQuan.Business;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OAnQuan.DataAccess
{
    public class PlayerDb
    {
        // We use the data source:
        static string connString = Services.ConnectionString;
        
        /// <summary>
        /// Creat the table of player
        /// </summary>
        public static void InitializePlayerTable()
        {
            using (SQLiteConnection db =
                new SQLiteConnection("Data Source=DatabaseOAQ.db; Version = 3; New = True; Compress = True; "))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT EXISTS T_Player " +
                    "(  [PlayerId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [Pseudo] text NOT NULL" +
                    ", [Password] text NOT NULL, [isAdmin] bigint NOT NULL, [FullName] text NOT NULL, [WinGameQty] bigint NOT NULL" +
                    ", [DrawGameQty] bigint NOT NULL, [LoseGameQty] bigint NOT NULL)";

                SQLiteCommand createTable = new SQLiteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        #region Sign In/ Sign Up
        /// <summary>
        /// Hash the password
        /// </summary>
        /// <param name="input">password inserted by user</param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }

        /// <summary>
        /// Insert player when the visitor creat a new account (!!!check if this pseudo already existe)
        /// </summary>
        /// <param name="pso">Pseudo taped by visitor</param>
        /// <param name="pass">Password taped by visitor</param>
        public static void InsertPlayer(string pseudo, string password, string fullName)
        {
            // create a new database connection:
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                // open the connection:
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // Lets insert something into our new table:
                cmd.CommandText = "INSERT INTO T_Player (Pseudo, Password, FullName, IsAdmin, IsEnabled, WinGameQty, DrawGameQty, LoseGameQty) " +
                    "VALUES (@pso, @pass, @fullName, @isAdmin, @isEnabled, @winGameQty, @drawGameQty, @loseGameQty);";
                cmd.Parameters.AddWithValue("@pso", pseudo);
                cmd.Parameters.AddWithValue("@pass", ComputeHash(password, new SHA256CryptoServiceProvider()));
                cmd.Parameters.AddWithValue("@fullName", fullName);
                cmd.Parameters.AddWithValue("@isAdmin", 0);
                cmd.Parameters.AddWithValue("@isEnabled", 1);
                cmd.Parameters.AddWithValue("@winGameQty", 0);
                cmd.Parameters.AddWithValue("@drawGameQty", 0);
                cmd.Parameters.AddWithValue("@loseGameQty", 0);
                // And execute this again ;D
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get player when user signs in/ or creat a new account
        /// </summary>
        /// <param name="pseudo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public Player GetPlayer(string pseudo, string password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    // First lets build a SQL-Query again:
                    cmd.CommandText = "SELECT * FROM T_Player WHERE Pseudo = @pso and Password = @pass";
                    cmd.Parameters.AddWithValue("@pso", pseudo);
                    cmd.Parameters.AddWithValue("@pass", ComputeHash(password, new SHA256CryptoServiceProvider()));

                    // Now the SQLiteCommand object can give us a DataReader-Object:
                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            return new Player()
                            {
                                PlayerId = (long)dataReader["PlayerId"],
                                Pseudo = pseudo,
                                Password = password,
                                FullName = (string)dataReader["FullName"],
                                IsAdmin = (long)dataReader["IsAdmin"],
                                IsEnabled = (long)dataReader["IsEnabled"],
                                WinGameQty = (long)dataReader["WinGameQty"],
                                DrawGameQty = (long)dataReader["DrawGameQty"],
                                LoseGameQty = (long)dataReader["LoseGameQty"],
                            };
                        }
                        else return null;
                    }
                } 
            }
        }
        #endregion

        /// <summary>
        /// GetPlayerIdFromPseudo
        /// </summary>
        /// <param name="pseudo"></param>
        /// <returns>playerId</returns>
        public static long GetPlayerIdFromPseudo(string pseudo)
        {
            long playerId = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PlayerId FROM T_Player WHERE Pseudo = @pseudo";
                    cmd.Parameters.AddWithValue("@pseudo", pseudo);

                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read()) // Read() returns true if there is still a result line to read
                        {
                            playerId = (long)dataReader["PlayerId"];
                        }
                    }
                }
            }
            return playerId;
        }

        /// <summary>
        /// Get player from pseudo
        /// </summary>
        /// <param name="pseudo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public Player GetPlayerFromPseudo(string pseudo)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // First lets build a SQL-Query again:
                cmd.CommandText = "SELECT * FROM T_Player WHERE Pseudo = @pso";
                cmd.Parameters.AddWithValue("@pso", pseudo);
                // Now the SQLiteCommand object can give us a DataReader-Object:
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    return new Player()
                    {
                        PlayerId = (long)dataReader["PlayerId"],
                        Pseudo = pseudo,
                        Password = (string)dataReader["Password"],
                        FullName = (string)dataReader["FullName"],
                        IsAdmin = (long)dataReader["IsAdmin"],
                        IsEnabled = (long)dataReader["IsEnabled"],
                        WinGameQty = (long)dataReader["WinGameQty"],
                        DrawGameQty = (long)dataReader["DrawGameQty"],
                        LoseGameQty = (long)dataReader["LoseGameQty"],
                    };
                }
                else return null;
            }
        }

        /// <summary>
        /// Get ranking with limit
        /// </summary>
        /// <param name="limit">number of best players to rank</param>
        /// <returns>pseudo, winGameQty, drawGameQty, loseGameQty</returns>
        public static List<Player> GetRankingPlayerListWithLimit(int limit)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // First lets build a SQL-Query again:
                cmd.CommandText = "SELECT * " +
                    "FROM T_Player GROUP BY Pseudo " +
                    "ORDER BY WinGameQty DESC, LoseGameQty, DrawGameQty DESC " +
                    "LIMIT @limit ";
                cmd.Parameters.AddWithValue("@limit", limit);

                // Now the SQLiteCommand object can give us a DataReader-Object:
                using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                {
                    // The SQLiteDataReader allows us to run through the result lines:
                    List<Player> listPlayer = new List<Player>();
                    while (dataReader.Read()) // Read() returns true if there is still a result line to read
                    {
                        listPlayer.Add(new Player()
                        {
                            PlayerId = (long)dataReader["PlayerId"],
                            Pseudo = (string)dataReader["Pseudo"],
                            Password = (string)dataReader["Password"],
                            FullName = (string)dataReader["FullName"],
                            IsAdmin = (long)dataReader["IsAdmin"],
                            IsEnabled = (long)dataReader["IsEnabled"],
                            WinGameQty = (long)dataReader["WinGameQty"],
                            DrawGameQty = (long)dataReader["DrawGameQty"],
                            LoseGameQty = (long)dataReader["LoseGameQty"]
                        });
                    }
                    return listPlayer;
                }
            }
        }

        /// <summary>
        /// Count the quantity of all players
        /// </summary>
        /// <returns>int</returns>
        public static int CountPlayer()
        {
            int count = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM T_Player";
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }

        /// <summary>
        /// Update the table Player in database
        /// </summary>
        /// <param name="player"></param>
        public static void UpdatePlayerDb(Player player)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE T_Player SET " +
                        "Pseudo = @pso, " +
                        "FullName = @full, " +
                        "IsAdmin = @ad, " +
                        "IsEnabled = @en, " +
                        "WinGameQty = @win, " +
                        "DrawGameQty = @draw, " +
                        "LoseGameQty = @lose " +
                    "WHERE PlayerId = @playerId";

                    cmd.Parameters.AddWithValue("@playerId", player.PlayerId);
                    cmd.Parameters.AddWithValue("@pso", player.Pseudo);
                    cmd.Parameters.AddWithValue("@full", player.FullName);
                    cmd.Parameters.AddWithValue("@ad", player.IsAdmin);
                    cmd.Parameters.AddWithValue("@en", player.IsEnabled);
                    cmd.Parameters.AddWithValue("@win", player.WinGameQty);
                    cmd.Parameters.AddWithValue("@draw", player.DrawGameQty);
                    cmd.Parameters.AddWithValue("@lose", player.LoseGameQty);
                    cmd.ExecuteNonQuery();
                }
                // We are ready, now lets cleanup and close our connection:
                conn.Close();
            }
        }

        #region Funtionnalities reserved for admins: GetAllPlayer, UpgradePlayerToAdmin, DeactivatePlayer, ReactivatePlayer, Search Player, SeeInfoOfEveryPlayer 
        /// <summary>
        /// Display the list of all players
        /// </summary>
        /// <returns></returns>
        public static List<Player> GetAllPlayer()
        {
            List<Player> listPlayer = new List<Player>();
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // First lets build a SQL-Query again:
                cmd.CommandText = "SELECT * FROM T_Player";

                // Now the SQLiteCommand object can give us a DataReader-Object:
                SQLiteDataReader dataReader = cmd.ExecuteReader();

                // The SQLiteDataReader allows us to run through the result lines:
                while (dataReader.Read()) // Read() returns true if there is still a result line to read
                {
                    listPlayer.Add(new Player()
                    {
                        PlayerId = (long)dataReader["PlayerId"],
                        Pseudo = (string)dataReader["Pseudo"],
                        Password = (string)dataReader["Password"],
                        FullName = (string)dataReader["FullName"],
                        IsAdmin = (long)dataReader["IsAdmin"],
                        IsEnabled = (long)dataReader["IsEnabled"],
                        WinGameQty = (long)dataReader["WinGameQty"],
                        DrawGameQty = (long)dataReader["DrawGameQty"],
                        LoseGameQty = (long)dataReader["LoseGameQty"]
                    });
                }
            }
            return listPlayer;
        }

        /// <summary>
        /// Admin upgrade a player to admin
        /// </summary>
        /// <param name="playerId">id of player</param>
        public static void UpgradePlayerToAdmin(long playerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE T_Player SET IsAdmin = 1 WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Admin deactivate a player
        /// </summary>
        /// <param name="playerId"></param>
        public static void DeactivatePlayer(long playerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE T_Player SET IsEnabled = 0 WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Admin can reactivate a player
        /// </summary>
        /// <param name="playerId"></param>
        public static void ReactivatePlayer(long playerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE T_Player SET IsEnabled = 1 WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
