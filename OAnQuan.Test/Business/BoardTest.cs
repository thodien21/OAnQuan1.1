using NUnit.Framework;
using OAnQuan.Business;
using System.Collections.Generic;
using System.Linq;

namespace OAnQuan.Test.Business
{
    [TestFixture]
    public static class BoardTest
    {
        [Test]
        public static void BoardContructor_Test()
        {
            var board = new Board();
            
            Assert.That(board, Is.Not.Null);
            Assert.That(board.SquaresList, Is.Not.Null);
            Assert.That(board.SquaresList.Count == 12);
            Assert.That(board.SquaresList[0].GetType().Equals(typeof(BigSquare)));
            Assert.That(board.SquaresList[1].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[2].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[3].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[4].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[5].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[6].GetType().Equals(typeof(BigSquare)));
            Assert.That(board.SquaresList[7].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[8].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[9].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[10].GetType().Equals(typeof(SmallSquare)));
            Assert.That(board.SquaresList[11].GetType().Equals(typeof(SmallSquare)));

            Assert.That(board.SquaresList[0].Tokens[0].GetType().Equals(typeof(BigToken)));
            Assert.That(board.SquaresList[1].Tokens[4].GetType().Equals(typeof(SmallToken)));
            Assert.That(board.SquaresList[1].Tokens.Count.Equals(5));

            Assert.That(board.SquaresList[0].Tokens.Count.Equals(1));
            Assert.That(board.SquaresList[6].Tokens.Count.Equals(1));
            for (int i=1; i<=5; i++)
            {
                Assert.That(board.SquaresList[i].Tokens.Count.Equals(5));
                Assert.That(board.SquaresList[i+6].Tokens.Count.Equals(5));
            }

            Assert.That(board.SquaresList[1], Is.Not.Null);
            Assert.AreEqual(board.SquaresList[1].Tokens.Count, 5);
            Assert.AreEqual(board.SquaresList[2].Tokens.Count, 5);
            Assert.AreEqual(board.SquaresList[3].Tokens.Count, 5);
            Assert.AreEqual(board.SquaresList[4].Tokens.Count, 5);
            Assert.AreEqual(board.SquaresList[0].Tokens.Count, 1);
        }
    }
}
