using System;
using System.Collections.Generic;
using System.Text;

namespace OAnQuan.Business
{
    public class BigSquare : Square
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BigSquare()
        {
            BigTokenQty = 1;
            Tokens.Add(new BigToken());
        }
    }
}
