using System;
using System.Collections.Generic;
using System.Text;

namespace OAnQuan.Business
{
    public class SmallSquare : Square
    {
        /// <summary>
        /// Dafault constructor
        /// </summary>
        public SmallSquare()
        {
            SmallTokenQty = 5;
            for (int i = 0; i < SmallTokenQty; i++)
                Tokens.Add(new SmallToken());
            Player = new Player();
        }
    }
}
