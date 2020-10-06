using OAnQuan.Business;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace OAnQuan.DataAccess
{
    public class SquareListDb
    {
        /// We use the data source:
        static string connString = Services.ConnectionString;

        /// <summary>
        /// Insert the square list of saved game
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
                for (int i = 0; i < 12; i++)
                {
                    Square square = board.SquaresList[i];
                    var squareIndex = i;
                    
                    var bigTokenQty = square.Tokens.Count(t => t.Value == 5);
                    var smallTokenQty = square.Tokens.Count(t => t.Value == 1);

                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO T_SquareList (SmallTokenQty, BigTokenQty, PlayerId, SquareIndex) VALUES (@smallTokenQty, @bigTokenQty, @playerId, @squareIndex) ;";
                        cmd.Parameters.AddWithValue("@smallTokenQty", smallTokenQty);
                        cmd.Parameters.AddWithValue("@bigTokenQty", bigTokenQty);
                        cmd.Parameters.AddWithValue("@playerId", playerId);
                        cmd.Parameters.AddWithValue("@squareIndex", squareIndex);
                        // And execute this again
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Update the square list of saved game
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
                for (int i = 0; i < 12; i++)
                {
                    Square square = board.SquaresList[i];
                    var squareIndex = i;
                    var bigTokenQty = square.Tokens.Count(t => t.Value == 5);
                    var smallTokenQty = square.Tokens.Count(t => t.Value == 1);

                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE T_SquareList SET SmallTokenQty = @smallTokenQty, BigTokenQty = @bigTokenQty WHERE PlayerId = @playerId AND SquareIndex = @squareIndex;";
                        cmd.Parameters.AddWithValue("@smallTokenQty", smallTokenQty);
                        cmd.Parameters.AddWithValue("@bigTokenQty", bigTokenQty);
                        cmd.Parameters.AddWithValue("@playerId", playerId);
                        cmd.Parameters.AddWithValue("@squareIndex", squareIndex);
                        // And execute this again
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static List<Square> GetSquareListDb(long playerId)
        {
            List<Square> squareList = new List<Square>();
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // create a new SQL command:
                SQLiteCommand cmd = conn.CreateCommand();

                // First lets build a SQL-Query again:
                cmd.CommandText = "SELECT * FROM T_SquareList WHERE PlayerId = @playerId";
                cmd.Parameters.AddWithValue("@playerId", playerId);

                // Now the SQLiteCommand object can give us a DataReader-Object:
                SQLiteDataReader dataReader = cmd.ExecuteReader();

                // The SQLiteDataReader allows us to run through the result lines:
                while (dataReader.Read()) // Read() returns true if there is still a result line to read
                {
                    squareList.Add(new Square()
                    {
                        SquareIndex = (long)dataReader["SquareIndex"],
                        SmallTokenQty= (long)dataReader["SmallTokenQty"],
                        BigTokenQty = (long)dataReader["BigTokenQty"]
                    });
                }
            }
            return squareList;
        }
    }
}
