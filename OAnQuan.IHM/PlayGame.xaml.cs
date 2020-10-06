using OAnQuan.Business;
using OAnQuan.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OAnQuan.IHM
{
    /// <summary>
    /// Logique d'interaction pour WindowGame.xaml
    /// </summary>
    public partial class PlayGame : Window
    {
        Random rand = new Random();
        Ellipse ellipse = null;
        List<Button> btnList = new List<Button>();
        List<Canvas> canList = new List<Canvas>();
        List<Button> btnListPool = new List<Button>();
        List<Canvas> canListPool = new List<Canvas>();
        
        Board board = (Services.NoveltyOfGame == NoveltyOfGame.NEW) ? new Board() : Services.Player.GetSavedGameFromDb();

        public const int u = 160;
        public PlayGame()
        {
            InitializeComponent();
            tblPseudoPlayer1.Text = board.PlayersList[0].Pseudo;
            tblPseudoPlayer2.Text = board.Player2Pseudo;

            //Set canlist and btnList for Board
            SetButtonListForBoard();
            SetCanvasListForBoard();

            //Set canlist and buttonList for Pools
            canListPool.Add(canvasPool1);
            canListPool.Add(canvasPool2);
            btnListPool.Add(buttonPool1);
            btnListPool.Add(buttonPool2);

            //Initialize the board at the beginning of the game
            SetBoardWithEllipes();
            //Set pools
            AddEllipsesInPool(1, board.PlayersList[0].Pool);
            AddEllipsesInPool(2, board.PlayersList[1].Pool);

            tblPlayerTurn.Text = "Tour de " + board.PlayersList[board.Turn - 1].Pseudo;
            Storyboard story = new Storyboard();
            foreach (var item in btnList)
            {
                // Animate the color and thickness of button's border when it's clicked.
                item.Click += delegate (object sender, RoutedEventArgs args)
                {
                    if ((board.SquaresList[btnList.IndexOf(item)].PlayerNumber != board.Turn || board.SquaresList[btnList.IndexOf(item)].TokenQty == 0) && board.ClickedSquares.Count == 0)
                        MessageBox.Show("La case choisie doit être non vide et se situer dans la rangée de joueur qui a le tour");
                    else
                    {
                        board.ClickedSquares.Add(btnList.IndexOf(item));
                        AnimateBorderWhenClicked(item);
                        if (board.ClickedSquares.Count == 1)
                        {
                            AnimateNeighborBorder(btnList[(btnList.IndexOf(item) + 1) % 12], story);
                            AnimateNeighborBorder(btnList[(btnList.IndexOf(item) + 11) % 12], story);
                        }
                        if (board.ClickedSquares.Count == 2)
                        {
                            Go();
                            story.Stop();//stop animation of neighbor squares
                            story.Children.Clear();//clear storyboard of neighbor squares
                            board.ClickedSquares.Clear();//Clear the list of clicked squares after each go
                        }
                    }
                    if (board.SquaresList[0].TokenQty == 0 && board.SquaresList[6].TokenQty == 0)
                        EndGame();
                    else if (board.SquaresList.Where(s => s.PlayerNumber == board.Turn).All(s => s.TokenQty == 0))//If the row of player is all empty, the pool suply 1 small token for each square
                    {
                        int smallTokenQtyInPool = board.PlayersList[board.Turn - 1].Pool.Count(t => t.Value == 1);
                        if (smallTokenQtyInPool >= 5)
                            Supply5SmallEllipses();
                        else
                            EndGame();
                    }
                };
            }


//This one is for changing the backgound color of the game
            cmbColors.SelectedIndex = 1;
            cmbColors.ItemsSource = typeof(Colors).GetProperties();
        }

        public void EndGame()
        {
            List<Token> tokensBackToPlayer1 = new List<Token>();
            List<Token> tokensBackToPlayer2 = new List<Token>();
            for (int i = 1; i < 6; i++)
            {
                tokensBackToPlayer1 = board.PlayersList[0].EatTokensInSquare(board.SquaresList[i]);
                tokensBackToPlayer2 = board.PlayersList[1].EatTokensInSquare(board.SquaresList[i + 6]);
            }
            UpdateEllipsesInBoard();//to remove ellipses in row
            AddEllipsesInPool(1, tokensBackToPlayer1);
            AddEllipsesInPool(2, tokensBackToPlayer2);
            Services.Player.UpdateResult(board);
            PlayerDb.UpdatePlayerDb(Services.Player);

            var declarationGameResult = "";
            switch (board.GetResult())
            {
                case Result.WIN:
                    declarationGameResult = Services.Player.Pseudo + " a gagné";
                    break;
                case Result.LOSE:
                    declarationGameResult = board.PlayersList[1].Pseudo + " a gagné";
                    break;
                case Result.DRAW:
                    declarationGameResult = "Partie nulle";
                    break;
            }

            MessageBoxResult result = MessageBox.Show(board.PlayersList[0].Pseudo + " a " + board.PlayersList[0].Score + " points\n"
                + board.PlayersList[1].Pseudo + " a " + board.PlayersList[1].Score + " points\n"
                + "\n" + declarationGameResult.ToUpper()
                + "\nVous voulez rejouer ?",
                "Résultat", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question, 
                MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Close();
                    Services.NoveltyOfGame = NoveltyOfGame.NEW;
                    PlayGame game = new PlayGame();
                    game.ShowDialog();
                    break;
                case MessageBoxResult.No:
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// Play a turn
        /// </summary>
        public void Go()
        {
            int squareId = board.ClickedSquares[0];
            int nextSquareId = board.ClickedSquares[1];
            Direction direction = Direction.UNKNOW;
            int tokenQty = board.SquaresList[squareId].TokenQty;
            List<Token> eatenTokens = new List<Token>();

            //determine the direction
            if ((squareId == 11 && nextSquareId == 0) || (squareId - nextSquareId == -1))
                direction = Direction.RIGHT;
            else if (squareId - nextSquareId == 1)
                direction = Direction.LEFT;
            else
                MessageBox.Show("Vous ne pouvez choisir que 2 cases de suite");
            //Play in chosen direction
            if (direction == Direction.LEFT || direction == Direction.RIGHT)
            {
                //share tokens and update ellipes
                while (squareId != 0 && squareId != 6 && tokenQty != 0)
                {
                    squareId = board.SmallStep(board.Turn, squareId, direction);//play small step and get squareId of next small step
                    tokenQty = board.SquaresList[squareId].TokenQty;//calculate token qty of next square

                    UpdateEllipsesInBoard();//Update the board after each small step
                }

                nextSquareId = board.CalculateNextSquareId(squareId, direction);//recalculate nextSquareId
                //while the next square is empty and its own next square is not empty, player earns tokens from its next square
                while (squareId != 0 && squareId != 6 && tokenQty == 0 && board.SquaresList[nextSquareId].TokenQty != 0)
                {
                    //Player eats tokens in next square
                    eatenTokens = board.PlayersList[board.Turn - 1].EatTokensInSquare(board.SquaresList[nextSquareId]);
                    UpdateEllipsesInBoard();
                    AddEllipsesInPool(board.Turn, eatenTokens);

                    //recalculate squareId and nextSquareId
                    squareId = board.CalculateNextSquareId(nextSquareId, direction);
                    tokenQty = board.SquaresList[squareId].TokenQty;
                    nextSquareId = board.CalculateNextSquareId(squareId, direction);
                }

                //Change turn
                board.Turn = (board.Turn == 1) ? 2 : 1;
                tblPlayerTurn.Text = "Tour de " + board.PlayersList[board.Turn - 1].Pseudo;
            }
        }


        #region animations of buttons
        /// <summary>
        /// Button will have a green and thicker border when clicked
        /// </summary>
        /// <param name="button"></param>
        public void AnimateBorderWhenClicked(Button button)
        {
            Storyboard story = new Storyboard();

            ColorAnimation anim = new ColorAnimation();
            anim.From = Colors.Gray;
            anim.To = Colors.Green;
            anim.BeginTime = TimeSpan.FromSeconds(0);
            anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            anim.AutoReverse = true;
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, button);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(Button.BorderBrush).(SolidColorBrush.Color)"));

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.From = new Thickness(1);
            thicknessAnimation.To = new Thickness(5);
            thicknessAnimation.BeginTime = TimeSpan.FromSeconds(0);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            thicknessAnimation.AutoReverse = true;
            story.Children.Add(thicknessAnimation);
            Storyboard.SetTarget(thicknessAnimation, button);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("(Button.BorderThickness)"));

            story.Begin();
        }

        /// <summary>
        /// Animate the neighbor squares still one of them is chosen
        /// </summary>
        /// <param name="button">button</param>
        /// <param name="story">storyboard</param>
        public void AnimateNeighborBorder(Button button, Storyboard story)
        {
            ColorAnimation anim = new ColorAnimation();
            anim.From = Colors.Gray;
            anim.To = Colors.Green;
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, button);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(Button.BorderBrush).(SolidColorBrush.Color)"));

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.From = new Thickness(1);
            thicknessAnimation.To = new Thickness(5);
            story.Children.Add(thicknessAnimation);
            Storyboard.SetTarget(thicknessAnimation, button);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("(Button.BorderThickness)"));

            story.Begin();
        }
        #endregion


        #region set up canvas and button List
        /// <summary>
        /// Add canlists in a list
        /// </summary>
        public void SetCanvasListForBoard()
        {
            canList.Add(canvas0);
            canList.Add(canvas1);
            canList.Add(canvas2);
            canList.Add(canvas3);
            canList.Add(canvas4);
            canList.Add(canvas5);
            canList.Add(canvas6);
            canList.Add(canvas7);
            canList.Add(canvas8);
            canList.Add(canvas9);
            canList.Add(canvas10);
            canList.Add(canvas11);
        }

        /// <summary>
        /// Add buttons in a list
        /// </summary>
        public void SetButtonListForBoard()
        {
            btnList.Add(button0);
            btnList.Add(button1);
            btnList.Add(button2);
            btnList.Add(button3);
            btnList.Add(button4);
            btnList.Add(button5);
            btnList.Add(button6);
            btnList.Add(button7);
            btnList.Add(button8);
            btnList.Add(button9);
            btnList.Add(button10);
            btnList.Add(button11);
        }
        #endregion


        #region update board interface with ellipses
        /// <summary>
        /// Visualize the board
        /// </summary>
        private void SetBoardWithEllipes()
        {
            //Set up big squares
            int n = 0;
            for (int k = 0; k < 2; k++)
            {
                long bigTokenQty = board.SquaresList[n].BigTokenQty;
                if(bigTokenQty ==1)
                    AddBigEllipse(n);//There is only maximum 1 big token in big square so don't need to use loop
                AddSmallEllipsesInBigSquare(n, board.SquaresList[n].SmallTokenQty);//Set big squares with small tokens
                n = n + 6;
            }

            //Set up small squares
            for (int i = 1; i < 6; i++)
                AddEllipsesInFirstRow(i, board.SquaresList[i].SmallTokenQty);
            for (int i = 7; i < 12; i++)
                AddEllipsesInSecondRow(i, board.SquaresList[i].SmallTokenQty);

            //Display the quantity of tokens in each square(button)
            for (int i = 0; i < 12; i++)
                btnList[i].Content = board.SquaresList[i].SmallTokenQty + board.SquaresList[i].BigTokenQty;
        }

        private void UpdateEllipsesInBoard()
        {
            //Display the quantity of tokens in each square(button)
            for (int i = 0; i < 12; i++)
            {
                int diffTokenQtyInSquare = canList[i].Children.Count - 1 - board.SquaresList[i].TokenQty;//The quantity of ellipse in canvas is canvas.Children.Count-1 because its first child is a button
                if (diffTokenQtyInSquare != 0)
                    DiffTokenQty(i, diffTokenQtyInSquare);
                btnList[i].Content = board.SquaresList[i].TokenQty;//the formular btnList[i].Content = canList[i].Children.Count is not used because canList.Count - ellipeQty = 1
            }
        }

        /// <summary>
        /// diffTokenQty = canList[n].Children.Count - board.SquaresList[n].TokenQty
        /// </summary>
        /// <param name="diff"></param>
        /// <param name="squareIndex"></param>
        public void DiffTokenQty(int squareIndex, int diffTokenQty)
        {
            if (diffTokenQty > 0)
                canList[squareIndex].Children.RemoveRange(1, diffTokenQty);
            else if (diffTokenQty < 0)
            {
                if (squareIndex == 0 || squareIndex == 6)
                    AddSmallEllipsesInBigSquare(squareIndex, Math.Abs(diffTokenQty));
                else if (squareIndex >= 1 && squareIndex <= 5)
                    AddEllipsesInFirstRow(squareIndex, Math.Abs(diffTokenQty));
                else if (squareIndex >= 7 && squareIndex <= 11)
                    AddEllipsesInSecondRow(squareIndex, Math.Abs(diffTokenQty));
            }
        }

        public void AddBigEllipse(int squareIndex)
        {
            ellipse = CreateAnEllipse(u / 2, u / 4);
            Canvas.SetLeft(ellipse, rand.Next(60 + squareIndex * u, 60 + squareIndex * u));
            Canvas.SetTop(ellipse, rand.Next(120, 2 * u - 160));
            canList[squareIndex].Children.Add(ellipse);
        }

        public void AddSmallEllipsesInBigSquare(int squareIndex, long smallTokenQty)
        {
            for (int j = 0; j < smallTokenQty; j++)
            {
                ellipse = CreateAnEllipse(20, 20);
                Canvas.SetLeft(ellipse, rand.Next(40 + squareIndex * u, 80 + squareIndex * u));
                Canvas.SetTop(ellipse, rand.Next(120, 2 * u - 160));
                canList[squareIndex].Children.Add(ellipse);
            }
        }

        /// <summary>
        /// Where squareIndex=1-5
        /// </summary>
        /// <param name="squareIndex"></param>
        public void AddEllipsesInFirstRow(int squareIndex, long smallTokenQty)
        {
            for (int j = 0; j < smallTokenQty; j++)
            {
                ellipse = CreateAnEllipse(20, 20);
                Canvas.SetLeft(ellipse, rand.Next(u * squareIndex + 50, u * squareIndex + u - 50));
                Canvas.SetTop(ellipse, rand.Next(u + 50, 2 * u - 50));
                canList[squareIndex].Children.Add(ellipse);
            }
        }

        /// <summary>
        /// Where squareIndex=7-11
        /// </summary>
        /// <param name="squareIndex"></param>
        public void AddEllipsesInSecondRow(int squareIndex, long smallTokenQty)
        {
            for (int j = 0; j < smallTokenQty; j++)
            {
                ellipse = CreateAnEllipse(20, 20);
                Canvas.SetLeft(ellipse, rand.Next(u * (12 - squareIndex) + 50, u * (12 - squareIndex) + u - 50));
                Canvas.SetTop(ellipse, rand.Next(50, u - 50));
                canList[squareIndex].Children.Add(ellipse);
            }
        }
        #endregion


        #region update pool and interface
        public void AddEllipsesInPool(int playerNumber, List<Token> eatenTokens)
        {
            int smallTokenQty = eatenTokens.Count(t => t.Value == 1);
            int bigTokenQty = eatenTokens.Count(t => t.Value == 5);

            AddBigEllipseInPool(playerNumber, bigTokenQty);
            AddSmallEllipsesInPool(playerNumber, smallTokenQty);
            btnListPool[playerNumber - 1].Content = board.PlayersList[playerNumber - 1].Score;
        }

        public void AddBigEllipseInPool(int playerNumber, int bigTokenQty)
        {
            for (int i = 0; i < bigTokenQty; i++)
            {
                ellipse = CreateAnEllipse(u / 2, u / 4);
                Canvas.SetLeft(ellipse, rand.Next(50, 150));

                if (playerNumber == 1)
                    Canvas.SetBottom(ellipse, rand.Next(25, 135));
                else
                    Canvas.SetTop(ellipse, rand.Next(25, 135));

                canListPool[playerNumber - 1].Children.Add(ellipse);
            }
        }

        public void AddSmallEllipsesInPool(int playerNumber, int smallTokenQty)
        {
            if (playerNumber == 1)
            {
                for (int j = 0; j < smallTokenQty; j++)
                {
                    ellipse = CreateAnEllipse(20, 20);
                    Canvas.SetLeft(ellipse, rand.Next(20, 200));
                    Canvas.SetBottom(ellipse, rand.Next(20, 160));
                    canvasPool1.Children.Add(ellipse);
                }
            }
            else if (playerNumber == 2)
            {
                for (int j = 0; j < smallTokenQty; j++)
                {
                    ellipse = CreateAnEllipse(20, 20);
                    Canvas.SetLeft(ellipse, rand.Next(0, 200));
                    Canvas.SetTop(ellipse, rand.Next(20, 160));
                    canvasPool2.Children.Add(ellipse);
                }
            }
        }

        public void Supply5SmallEllipses()
        {
            //Rank tokens in ascendant order of value
            List<Token> list = new List<Token>();
            list.AddRange(board.PlayersList[board.Turn - 1].Pool.OrderBy(t => t.Value));
            board.PlayersList[board.Turn - 1].Pool.Clear();
            board.PlayersList[board.Turn - 1].Pool.AddRange(list);

            //Remove 5 small tokens and refresh the display of Pool button
            board.PlayersList[board.Turn - 1].Pool.RemoveRange(0, 5);
            btnListPool[board.Turn - 1].Content = board.PlayersList[board.Turn - 1].Pool.Count;

            //clear and refill canvas with new tokenslist in popl
            canListPool[board.Turn - 1].Children.RemoveRange(1, canListPool[board.Turn - 1].Children.Count);
            AddEllipsesInPool(board.Turn, board.PlayersList[board.Turn - 1].Pool);

            //Add 1 token in each square of player's row as well as update interface 
            if (board.Turn == 1)
            {
                for (int j = 1; j < 6; j++)
                {
                    board.SquaresList[j].Tokens.Add(new SmallToken());
                    AddEllipsesInFirstRow(j, 1);
                }
            }
            else
            {
                for (int j = 7; j < 12; j++)
                {
                    board.SquaresList[j].Tokens.Add(new SmallToken());
                    AddEllipsesInSecondRow(j, 1);
                }
            }
        }
        #endregion


        /// <summary>
        /// Creat a new ellipse
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Ellipse CreateAnEllipse(int height, int width)
        {
            SolidColorBrush fillBrush = new SolidColorBrush() { Color = Colors.Red };
            SolidColorBrush borderBrush = new SolidColorBrush() { Color = Colors.Black };

            return new Ellipse()
            {
                Height = height,
                Width = width,
                StrokeThickness = 1,
                Stroke = borderBrush,
                Fill = fillBrush
            };
        }

        private void BtnSaveGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(board.PlayersList[0].Pseudo +" a " + board.PlayersList[0].Score + " points\n" 
                + board.PlayersList[1].Pseudo + " a " + board.PlayersList[1].Score + " points\n"
                + "\nVous voulez savegarder la partie ?",
                "Sauvegarder",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.OK);
            switch (result)
            {
                case MessageBoxResult.OK:
                    Services.Player.SaveOrUpdateGame(board.PlayersList[1].Pseudo, board.Turn, board);
                    MessageBox.Show("Partie sauvegardée.");
                    this.Close();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void BtnExitGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(board.PlayersList[0].Pseudo + " a " + board.PlayersList[0].Score + " points\n"
                + board.PlayersList[1].Pseudo + " a " + board.PlayersList[1].Score + " points\n"
                + "\nVous voulez vraiment quitter la partie sans sauvegarder?",
                "Quitter",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.OK);
            switch (result)
            {
                case MessageBoxResult.OK:
                    this.Close();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        /// <summary>
        /// Change background color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbColors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Color selectedColor = (Color)(cmbColors.SelectedItem as PropertyInfo).GetValue(null, null);
            this.Background = new SolidColorBrush(selectedColor);
        }
    }
}
