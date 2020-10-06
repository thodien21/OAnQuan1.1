using OAnQuan.Business;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace OAnQuan.DataAccess
{
    public class PoolDb
    {
        // We use the data source:
        static string connString = Services.ConnectionString;

        /// <summary>
        /// Insert the pool's player of saved game
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerId"></param>
        public static void Save(long playerId, Board board)
        {
            // create a new database connection:
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                // open the connection:
                conn.Open();
                // create a new SQL command:
                for (int i = 0; i < 2; i++)
                {
                    List<Token> pool = board.PlayersList[i].Pool;
                    long playerNumber = i + 1;
                    var bigTokenQty = pool.Count(t => t.Value == 5);
                    var smallTokenQty = pool.Count(t => t.Value == 1);

                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO T_Pool (SmallTokenQty, BigTokenQty, PlayerId, PlayerNumber) VALUES (@smallTokenQty, @bigTokenQty, @playerId, @playerNumber) ;";
                        cmd.Parameters.AddWithValue("@smallTokenQty", smallTokenQty);
                        cmd.Parameters.AddWithValue("@bigTokenQty", bigTokenQty);
                        cmd.Parameters.AddWithValue("@playerId", playerId);
                        cmd.Parameters.AddWithValue("@playerNumber", playerNumber);
                        // And execute this again
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Update the pool's player of saved gamee
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerId"></param>
        public static void Update(long playerId, Board board)
        {
            // create a new database connection:
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                // open the connection:
                conn.Open();
                // create a new SQL command:
                for (int i = 0; i < 2; i++)
                {
                    List<Token> pool = board.PlayersList[i].Pool;
                    long playerNumber = i + 1;
                    var bigTokenQty = pool.Count(t => t.Value == 5);
                    var smallTokenQty = pool.Count(t => t.Value == 1);

                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE T_Pool SET SmallTokenQty = @smallTokenQty, BigTokenQty = @bigTokenQty WHERE PlayerId = @playerId AND PlayerNumber = @playerNumber;";
                        cmd.Parameters.AddWithValue("@smallTokenQty", smallTokenQty);
                        cmd.Parameters.AddWithValue("@bigTokenQty", bigTokenQty);
                        cmd.Parameters.AddWithValue("@playerId", playerId);
                        cmd.Parameters.AddWithValue("@playerNumber", playerNumber);
                        // And execute this again
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static List<Square> GetPoolListDb(long playerId)
        {
            List<Square> poolList = new List<Square>();
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // First lets build a SQL-Query again:
                cmd.CommandText = "SELECT * FROM T_Pool WHERE PlayerId = @playerId";
                cmd.Parameters.AddWithValue("@playerId", playerId);

                // Now the SQLiteCommand object can give us a DataReader-Object:
                SQLiteDataReader dataReader = cmd.ExecuteReader();

                // The SQLiteDataReader allows us to run through the result lines:
                while (dataReader.Read()) // Read() returns true if there is still a result line to read
                {
                    poolList.Add(new Square()
                    {
                        PlayerNumber = (long)dataReader["PlayerNumber"],
                        SmallTokenQty = (long)dataReader["SmallTokenQty"],
                        BigTokenQty = (long)dataReader["BigTokenQty"]
                    });
                }
            }
            return poolList;
        }
    }
}
