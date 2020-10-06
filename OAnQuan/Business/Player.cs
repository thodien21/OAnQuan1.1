using OAnQuan.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OAnQuan.Business
{
    /// <summary>
    /// Player.
    /// </summary>
    public class Player
    {
        #region properties
        public string FullName { get; set; }

        public string Pseudo { get; set; }
        public long PlayerId { get; internal set; }
        public string Password { get; internal set; }

        public long IsAdmin { get; set; }
        public string IsAdminString => (IsAdmin == 1) ? "Oui" : "Non";

        public long IsEnabled { get; set; }
        public string IsEnabledString => (IsEnabled == 1) ? "Oui" : "Non";

        public long WinGameQty { get; set; }
        public long DrawGameQty { get; set; }
        public long LoseGameQty { get; set; }
        public long TotalGameQty => WinGameQty + DrawGameQty + LoseGameQty;
        public long Ranking => GetOwnRanking();

        /// <summary>
        /// Pool of tokens actually earned in the game
        /// </summary>
        public List<Token> Pool { get; set; }

        /// <summary>
        /// actual score of player in the game
        /// </summary>
        public int Score => Pool.Sum(p => p.Value);
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pseudo">Pseudo.</param>
        /// <param name="password">Password.</param>
        public Player(string pseudo)
        {
            Pseudo = pseudo;
            Pool = new List<Token>();
        }

        public Player()
        {
            Pool = new List<Token>();
        }
        #endregion

        #region functionalities for player
        //// <summary>
        /// Get ranking of own player
        /// </summary>
        /// <returns>int</returns>
        public int GetOwnRanking()
        {
            var ownRanking = 0;
            var count = PlayerDb.CountPlayer();
            var playerListRanking = PlayerDb.GetRankingPlayerListWithLimit(count);
            for (int i = 0; i < count; i++)
            {
                if (playerListRanking[i].Pseudo == Pseudo)
                {
                    ownRanking = i+1;
                    break;
                }
            }
            return ownRanking;
        }

        public List<String> GetGroupedInfo()
        {
            var listInfo= new List<string>();
            listInfo.Add(Pseudo);
            listInfo.Add(FullName);
            return listInfo;
        }

        /// <summary>
        /// If it's the first time player saves the game -> Save board, if not the first time-> Update board
        /// </summary>
        /// <param name="turn">turn of which player?</param>
        /// <param name="player2Pseudo">pseudo of player number 2</param>
        /// <param name="playerId">player identity</param>
        public void SaveOrUpdateGame(string player2Pseudo, long turn, Board board)
        {
            bool contains = BoardDb.CheckIfBoardDbContainsPlayerId(PlayerId);
            if (contains)
            {
                BoardDb.Update(PlayerId, player2Pseudo, turn);
                SquareListDb.Update(PlayerId, board);
                PoolDb.Update(PlayerId, board);
            }
            else
            {
                BoardDb.Save(PlayerId, player2Pseudo, turn);
                SquareListDb.Save(PlayerId, board);
                PoolDb.Save(PlayerId, board);
            }
        }

        public Board GetSavedGameFromDb()
        {
            Board board = new Board();
            board.Turn = BoardDb.GetTurnFromDb(PlayerId);
            board.Player2Pseudo = BoardDb.GetPlayer2PseudoFromDb(PlayerId);
            board.PlayersList = new List<Player> { this, new Player(board.Player2Pseudo) };

            //recover squareList with SmallTokenQty and BigTokenQty by reserving all other properties
            var squaresListDb = SquareListDb.GetSquareListDb(PlayerId);
            for (int i = 0; i < 12; i++)
            {
                board.SquaresList[i].Tokens = new List<Token>();
                board.SquaresList[i].SmallTokenQty = squaresListDb[i].SmallTokenQty;
                board.SquaresList[i].BigTokenQty = squaresListDb[i].BigTokenQty;
                for (int j = 0; j < board.SquaresList[i].SmallTokenQty; j++)
                    board.SquaresList[i].Tokens.Add(new SmallToken());
                for (int j = 0; j < board.SquaresList[i].BigTokenQty; j++)
                    board.SquaresList[i].Tokens.Add(new BigToken());
            }

            var poolList = PoolDb.GetPoolListDb(PlayerId);
            //Recover pool of each player
            for (int i= 0; i <2; i++)
            {
                board.PlayersList[i].Pool = new List<Token>();
                for (int j = 0; j < poolList[i].SmallTokenQty; j++)
                    board.PlayersList[i].Pool.Add(new SmallToken());
                for (int j = 0; j < poolList[i].BigTokenQty; j++)
                    board.PlayersList[i].Pool.Add(new BigToken());
            }
            return board;
        }

        /// <summary>
        /// Update result in database
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerId"></param>
        public void UpdateResult(Board board)
        {
            var result = board.GetResult();
            switch (result)
            {
                case Result.WIN:
                    this.WinGameQty++;
                    break;
                case Result.DRAW:
                    this.DrawGameQty++;
                    break;
                case Result.LOSE:
                    this.LoseGameQty++;
                    break;
                default: break;
            }
        }

        public List<Token> EatTokensInSquare(Square square)
        {
            List<Token> earnedTokens = new List<Token>();
            earnedTokens.AddRange(square.Tokens);

            square.Tokens.Clear();//Remove tokens in eaten square
            Pool.AddRange(earnedTokens);//Add eaten tokens in player's pool

            return earnedTokens;
        }
        #endregion

        #region functionalities for Admin
        private void GetAllPlayer()
        {
            if(IsAdmin == 1)
            {
                PlayerDb.GetAllPlayer();
            }
            else throw new ArgumentOutOfRangeException(nameof(PlayerId), "Cette fonctionnalité n'est réservée qu'au administrateur");
        }

        private void UpgradePlayerToAdmin(long playerId)
        {
            if (IsAdmin == 1)
            {
                PlayerDb.UpgradePlayerToAdmin(playerId);
            }
            else throw new ArgumentOutOfRangeException(nameof(PlayerId), "Cette fonctionnalité n'est réservée qu'au administrateur");
        }

        private void DeactivatePlayer(long playerId)
        {
            if (IsAdmin == 1)
            {
                PlayerDb.DeactivatePlayer(playerId);
            }
            else throw new ArgumentOutOfRangeException(nameof(PlayerId), "Cette fonctionnalité n'est réservée qu'au administrateur");
        }

        private void ReactivatePlayer(long playerId)
        {
            if (IsAdmin == 1)
            {
                PlayerDb.ReactivatePlayer(playerId);
            }
            else throw new ArgumentOutOfRangeException(nameof(PlayerId), "Cette fonctionnalité n'est réservée qu'au administrateur");
        }
        #endregion
    }
}
