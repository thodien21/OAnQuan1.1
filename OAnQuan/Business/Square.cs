using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAnQuan.Business
{
    public class Square
    {
        public long SmallTokenQty { get; set; }
        public long BigTokenQty { get; set; }
        public long SquareIndex { get; set; }
        public List<Token> Tokens { get; set; }

        public Player Player { get; set; }

        public long PlayerNumber { get; set; }

        public int TokenQty => Tokens.Count;

        public Square()
        {
            Tokens = new List<Token>();
        }
    }
}
