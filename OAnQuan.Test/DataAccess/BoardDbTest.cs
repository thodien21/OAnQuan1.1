using NUnit.Framework;
using OAnQuan.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAnQuan.Test.DataAccess
{
    class BoardDbTest
    {
        [TestFixture]
        public static class DbPlayerTest
        {
            [Test]
            public static void SaveGame_Test()
            {
                Assert.AreEqual(PlayerDb.GetPlayerIdFromPseudo(""), 1);
                //BoardDb.SaveOrUpdate(2, "Lai thu", 6);
            }
        }
    }
}
