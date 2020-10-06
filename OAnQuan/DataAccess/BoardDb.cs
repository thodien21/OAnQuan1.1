using OAnQuan.Business;
using System;
using System.Data.SQLite;

namespace OAnQuan.DataAccess
{
    public static class BoardDb
    {
        // We use the data source:
        static string connString = Services.ConnectionString;

        /// <summary>
        /// Check if the player has saved the game
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static bool CheckIfBoardDbContainsPlayerId(long playerId)
        {
            bool contains = false;
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT PlayerId FROM T_Board WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read()) // Read() returns true if there is still a result line to read
                            contains = true;
                    }
                }
            }
            return contains;
        }

        /// <summary>
        /// Save board
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="player2Pseudo"></param>
        /// <param name="playerId"></param>
        public static void Save(long playerId, string player2Pseudo, long turn)
        {
            // create a new database connection:
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                // open the connection:
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO T_Board (Turn, Player2Pseudo, PlayerId) VALUES (@turn, @player2Pseudo, @playerId) ;";
                    cmd.Parameters.AddWithValue("@turn", turn);
                    cmd.Parameters.AddWithValue("@player2Pseudo", player2Pseudo);
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    // And execute this again
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Update board
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="player2Pseudo"></param>
        /// <param name="playerId"></param>
        public static void Update(long playerId, string player2Pseudo, long turn)
        {
            // create a new database connection:
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                // open the connection:
                conn.Open();
                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE T_Board SET Turn = @turn, Player2Pseudo = @player2Pseudo WHERE PlayerId = @playerId;";
                    cmd.Parameters.AddWithValue("@turn", turn);
                    cmd.Parameters.AddWithValue("@player2Pseudo", player2Pseudo);
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    // And execute this again
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static public Board GetSavedBoardFromDb(long playerId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    // First lets build a SQL-Query again:
                    cmd.CommandText = "SELECT * FROM T_Board WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    // Now the SQLiteCommand object can give us a DataReader-Object:
                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            return new Board()
                            {
                                Turn = Convert.ToInt32((long)dataReader["Turn"]),
                                Player2Pseudo = (string)dataReader["Player2Pseudo"]
                            };
                        }
                        else return null;
                    } 
                }
            }
        }
       
        /// <summary>
        /// Get pseudo of player 2
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        static public string GetPlayer2PseudoFromDb(long playerId)
        {
            string player2Pseudo = "tata";
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    // First lets build a SQL-Query again:
                    cmd.CommandText = "SELECT * FROM T_Board WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    // Now the SQLiteCommand object can give us a DataReader-Object:
                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                            player2Pseudo = (string)dataReader["Player2Pseudo"];
                    }
                }
            }
            return player2Pseudo;
        }

        static public int GetTurnFromDb(long playerId)
        {
            int turn = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    // First lets build a SQL-Query again:
                    cmd.CommandText = "SELECT * FROM T_Board WHERE PlayerId = @playerId";
                    cmd.Parameters.AddWithValue("@playerId", playerId);

                    // Now the SQLiteCommand object can give us a DataReader-Object:
                    using (SQLiteDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                            turn = Convert.ToInt32((long)dataReader["Turn"]);
                    }
                }
            }
            return turn;
        }
    }
}
