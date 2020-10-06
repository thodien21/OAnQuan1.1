using NUnit.Framework;
using OAnQuan.Business;
using OAnQuan.DataAccess;
using System.Collections.Generic;

namespace OAnQuan.Test
{
    [TestFixture]
    public static class CompleteGameTest
    {
        /// <summary>
        /// Test of each turn
        /// </summary>
        /// <param name="board">the board</param>
        /// <param name="player">the player</param>
        /// <param name="squareId">the square Id</param>
        /// <param name="direction">the choosen direction</param>
        /// <param name="listGotten">the expected list of square after turn</param>
        /// <param name="score">score of player after turn</param>
        static void Turn(Board board, int playerNumber, int squareId, Direction direction, List<int> listGotten, int earnedtokenQty)
        {
            board.Go(playerNumber, squareId, direction);
            Assert.AreEqual(board.PlayersList[playerNumber -1].Pool.Count, earnedtokenQty);
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(board.SquaresList[i].Tokens.Count, listGotten[i]);
            }
        }

        [Test]
        public static void SimpleGame_Test()
        {
            // Setup game
            //setup board   
            Board board = new Board();
            //Services.Player = PlayerDb.GetPlayerFromPseudo("");
            Services.Player.Pseudo = "";
            Services.PseudoPlayer2 = "HaloInvitée";

            var player1 = board.PlayersList[0];
            var player2 = board.PlayersList[1];
            Assert.AreEqual(board.PlayersList[0].Pseudo, "");
            Assert.AreEqual(board.PlayersList[1].Pseudo, "HaloInvitée");

            var mySquares = board.SquaresList;
            Assert.AreEqual(mySquares.Count, 12);
            Assert.AreEqual(board.PlayersList.Count, 2);

            //test player
            for (int i=1; i<=5; i++)
            {
                Assert.AreEqual(mySquares[i].PlayerNumber, 1);
                Assert.AreEqual(mySquares[i+6].PlayerNumber, 2);
            }

            //test number of tokens in each square
            for (int i=1; i<=5; i++)
            {
                Assert.AreEqual(mySquares[i].Tokens.Count, 5);
                Assert.AreEqual(mySquares[i+6].Tokens.Count, 5);
            }
            Assert.AreEqual(mySquares[0].Tokens.Count, 1);
            Assert.AreEqual(mySquares[6].Tokens.Count, 1);

            Assert.That(board.PlayersList[0], Is.Not.Null);
            Assert.That(board.PlayersList[0].Pseudo, Is.Not.Null);
            Assert.AreEqual(board.PlayersList[0].Pool.Count, 0);
            Assert.AreEqual(player2.Pool.Count, 0);

            //expected lists after turns
            var list1 = new List<int> { 2, 0, 0, 6, 6, 6, 2, 0, 6, 6, 6, 6 };
            var list2 = new List<int> { 0, 0, 1, 7, 7, 7, 3, 1, 0, 6, 6, 6 };
            var list3 = new List<int> { 0, 0, 5, 3, 10, 2, 6, 2, 3, 1, 0, 9 };
            var list4 = new List<int> { 2, 1, 8, 6, 2, 1, 9, 2, 5, 3, 2, 0 };
            var list5 = new List<int> { 3, 0, 9, 0, 1, 3, 11, 1, 7, 5, 0, 0 };
            var list6 = new List<int> { 4, 1, 10, 0, 0, 3, 11, 1, 7, 0, 1, 1 };
            var list7 = new List<int> { 5, 0, 11, 1, 1, 0, 11, 1, 0, 0, 2, 0 };
            var list8 = new List<int> { 0, 0, 0, 1, 1, 0, 11, 0, 1, 0, 0, 0 };
            var list9 = new List<int> { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0 };

            //Assert the list of square after turn and the number of tokens in each player's pool 
            Turn(board, 1, 1, Direction.RIGHT, list1, 6);
            
            Turn(board, 2, 8, Direction.LEFT, list2, 2);
            Turn(board, 1, 3, Direction.LEFT, list3, 9);
            Turn(board, 2, 7, Direction.LEFT, list4, 2);
            Turn(board, 1, 4, Direction.RIGHT, list5, 10);
            
            Turn(board, 2, 9, Direction.RIGHT, list6, 3);
            Turn(board, 1, 5, Direction.LEFT, list7, 17);

            Turn(board, 2, 7, Direction.RIGHT, list8, 21);
            
            Turn(board, 1, 3, Direction.RIGHT, list9, 29);
            
            //END OF GAME: Assert the score of each player
            Assert.AreEqual(player1.Score, 33);
            Assert.AreEqual(player2.Score, 25);

            board.GetResult();

            //check score
            Assert.AreEqual(player1.Score, 35);
            Assert.AreEqual(player2.Score, 25);

            // check pool (qty of tokens)
            Assert.AreEqual(player1.Pool.Count, 31);
            Assert.AreEqual(player2.Pool.Count, 21);

            //check result (win/draw/lose)
            Assert.AreEqual(board.GetResult(), Result.WIN);
        }
    }
}
