using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        private int squareSize;
        public MainWindow()
        {
            InitializeComponent();
            NewGame();
            newGameButton.Click += new RoutedEventHandler(NewGameButtonClick);
            saveButton.Click += new RoutedEventHandler(SaveButtonClick);
            loadButton.Click += new RoutedEventHandler(LoadButtonClick);
            pauseButton.Click += new RoutedEventHandler(PauseButtonClick);
            boardCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(AddPawn);
            this.squareSize = 60;
            this.DataContext = this.game;
            Console.WriteLine(game.BlackPawns);
        }
        #region event handlers
        private void NewGameButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Êtes vous sûr de vouloir lancer une nouvelle partie?", "Nouvelle partie", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NewGame();
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        { 
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Sauver une partie";
            openFileDialog.FileName = "game";
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Text documents (.json)|*.json";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                game.save(filename);
            }
        }

        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Charger une partie";
            openFileDialog.FileName = "game";
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Text documents (.json)|*.json";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                game.load(filename);
            }
        }

        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddPawn(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(boardCanvas);
            int column = (int)pos.X / this.squareSize;
            int line = (int) pos.Y / this.squareSize;
            Console.WriteLine($"Trying to add a pawn at square {column}, {line}");
            if (this.game.playMove(column, line, this.game.CurrentPlayer))
            {
                this.DrawBoard();
            }
            else
            {
                Console.WriteLine($"{(this.game.CurrentPlayer ? "White" : "Black")} cannot play.");
            }
            
        }
        #endregion
        /// <summary>
        /// Reset the game.
        /// </summary>
        private void NewGame()
        {
            this.game = new Game();
            DrawBoard();
        }
        /// <summary>
        /// Draws the board in the window's canvas.
        /// </summary>
        private void DrawBoard()
        {
            Tile[,] gameBoard = this.game.Board;
            int min = (int) Math.Min(this.boardCanvas.Width, this.boardCanvas.Height);
            SolidColorBrush brush = new SolidColorBrush();

            brush.Color = Color.FromRgb(0, 128, 0);
            this.boardCanvas.Width = min;
            this.boardCanvas.Height = min;

            this.squareSize = min / 8;

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    brush.Color = Color.FromRgb(0, 128, 0);
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = this.squareSize;
                    rectangle.Height = this.squareSize;
                    rectangle.Fill = brush;
                    rectangle.Stroke = Brushes.Black;
                    this.boardCanvas.Children.Add(rectangle);
                    Canvas.SetTop(rectangle, i * this.squareSize);
                    Canvas.SetLeft(rectangle, j * this.squareSize);
                    if (gameBoard[i, j].Value == 0)
                    {
                        brush.Color = Color.FromRgb(255, 255, 255);
                        Ellipse circle = new Ellipse();
                        circle.Width = this.squareSize;
                        circle.Height = this.squareSize;
                        circle.Fill = brush;
                        circle.Stroke = Brushes.Black;
                        this.boardCanvas.Children.Add(circle);
                        Canvas.SetTop(circle, i * this.squareSize);
                        Canvas.SetLeft(circle, j * this.squareSize);
                    }
                    if (gameBoard[i, j].Value == 1)
                    {
                        brush.Color = Color.FromRgb(0, 0, 0);
                        Ellipse circle = new Ellipse();
                        circle.Width = this.squareSize;
                        circle.Height = this.squareSize;
                        circle.Fill = brush;
                        circle.Stroke = Brushes.White;
                        this.boardCanvas.Children.Add(circle);
                        Canvas.SetTop(circle, i * this.squareSize);
                        Canvas.SetLeft(circle, j * this.squareSize);
                    }
                }
            }
        }
    }
}
