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

        /// <summary>
        /// UI entry point.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.game = new Game(this);
            newGameButton.Click += new RoutedEventHandler(NewGameButtonClick);
            saveButton.Click += new RoutedEventHandler(SaveButtonClick);
            loadButton.Click += new RoutedEventHandler(LoadButtonClick);
            pauseButton.Click += new RoutedEventHandler(PauseButtonClick);
            //undoButton.Click += new RoutedEventHandler(Undo);
            toggleHelp.Checked += new RoutedEventHandler(ToggleHelp);
            boardCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(AddPawn);
            this.squareSize = 60;
            this.DataContext = this.game;
            toggleHelp.IsChecked = true;
            NewGame();

        }
        #region event handlers

        /// <summary>
        /// Revert back to the last turn if it is possible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo(object sender, RoutedEventArgs e)
        {
            if(!game.Undo())
            {
                MessageBox.Show("You cannot come back", "Undoing impossible", MessageBoxButton.OK);
            }
            else
            {
                DrawBoard();
            }
        }

        /// <summary>
        /// Enable/Disable playable tiles display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleHelp(object sender, RoutedEventArgs e)
        {
            DrawBoard();
        }

        /// <summary>
        /// Create a new game and refresh the interface's canvas and labels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to start a new game ?", "New Game", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                NewGame();
            }
        }

        /// <summary>
        /// Open a SaveFileDialog to allow players to choose a location
        /// where the game will be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            PauseButtonClick(null, null);
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Title = "Save the game";
            openFileDialog.FileName = "game";
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Text documents (.json)|*.json";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                game.Save(filename);
            }
        }

        /// <summary>
        /// Show an OpenFileDialog to let user choose a JSON file
        /// to load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            PauseButtonClick(null, null);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Load a game";
            openFileDialog.FileName = "game";
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Text documents (.json)|*.json";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                game.Load(filename);
                DrawBoard();
            }
        }

        /// <summary>
        /// Pause the game. No pawn can be placed and players' timers are stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            game.Pause();
            pauseButton.Content = game.Paused ? "Resume" : "Pause"; 
        }

        /// <summary>
        /// Try to add a pawn to the board follwing the click position
        /// in the canvas.
        /// </summary>
        private void AddPawn(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(boardCanvas);
            int column = (int)pos.X / this.squareSize;
            int line = (int) pos.Y / this.squareSize;
            //Console.WriteLine($"Trying to add a pawn at square {column}, {line}");
            if (this.game.playMove(column, line, this.game.CurrentPlayer))
            {
                this.DrawBoard();
            }
            else
            {
                //Console.WriteLine($"{(this.game.CurrentPlayer ? "White" : "Black")} player cannot play.");
            }
            
        }
        #endregion

        /// <summary>
        /// Reset the game.
        /// </summary>
        private void NewGame()
        {
            game.NewGame();
            DrawBoard();
        }

        /// <summary>
        /// Called when the game is over.
        /// </summary>
        /// <param name="message"></param>
        public void GameOver(string message)
        {   
            DrawBoard();
            message += "\n";
            message += $"The {game.GetWinner()} player won the game.\n";
            message += "Do you want to play again?";
            MessageBoxResult result = MessageBox.Show(message, "Game Over", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                NewGame();
            }
            else
            {
                // Used to avoid mutltithreading issue. An InvalidOperationException is thrown if
                // it is not secured with Dispatcher.Invoke.
                this.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        }

        /// <summary>
        /// Draws the board in the window's canvas.
        /// </summary>
        private void DrawBoard()
        {
            // Used to avoid multithreading issue
            this.Dispatcher.Invoke(() =>
            {
                boardCanvas.Children.Clear();
                Tile[,] gameBoard = this.game.Board;
                int min = (int)Math.Min(this.boardCanvas.Width, this.boardCanvas.Height);
                SolidColorBrush brush = new SolidColorBrush();

                brush.Color = Color.FromRgb(0, 128, 0);
                this.boardCanvas.Width = min;
                this.boardCanvas.Height = min;

                this.squareSize = min / 8;

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Width = this.squareSize;
                        rectangle.Height = this.squareSize;
                        rectangle.Fill = Brushes.Green;

                        if ((bool)toggleHelp.IsChecked)
                        {
                            foreach (var tile in game.Playable)
                            {

                                if (tile.X == i && tile.Y == j)
                                {
                                    rectangle.Fill = Brushes.Gray;
                                }
                            }
                        }

                        rectangle.Stroke = Brushes.Black;
                        this.boardCanvas.Children.Add(rectangle);
                        Canvas.SetTop(rectangle, i * this.squareSize);
                        Canvas.SetLeft(rectangle, j * this.squareSize);
                        if (gameBoard[i, j].Value == 0)
                        {
                            Ellipse circle = new Ellipse();
                            circle.Width = this.squareSize * 0.9;
                            circle.Height = this.squareSize * 0.9;
                            circle.Fill = Brushes.Black;
                            circle.Stroke = Brushes.Black;
                            this.boardCanvas.Children.Add(circle);
                            Canvas.SetTop(circle, i * this.squareSize - (circle.Width - this.squareSize) / 2);
                            Canvas.SetLeft(circle, j * this.squareSize - (circle.Width - this.squareSize) / 2);
                        }
                        if (gameBoard[i, j].Value == 1)
                        {
                            Ellipse circle = new Ellipse();
                            circle.Width = this.squareSize * 0.9;
                            circle.Height = this.squareSize * 0.9;
                            circle.Fill = Brushes.White;
                            circle.Stroke = Brushes.White;
                            this.boardCanvas.Children.Add(circle);
                            Canvas.SetTop(circle, i * this.squareSize - (circle.Width - this.squareSize) / 2);
                            Canvas.SetLeft(circle, j * this.squareSize - (circle.Width - this.squareSize) / 2);
                        }
                    }
                }
            });
        }
    }
}
