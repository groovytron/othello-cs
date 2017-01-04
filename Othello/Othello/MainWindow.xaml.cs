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
        public MainWindow()
        {
            InitializeComponent();
            game = new Game();
            DrawBoard();
        }

        /// <summary>
        /// Draws the board in the window's canvas.
        /// </summary>
        private void DrawBoard()
        {
            int[,] gameBoard = game.Board;
            int min = (int) Math.Min(boardCanvas.Width, boardCanvas.Height);
            SolidColorBrush brush = new SolidColorBrush();

            brush.Color = Color.FromRgb(0, 128, 0);
            boardCanvas.Width = min;
            boardCanvas.Height = min;

            int size = min / 8;

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    brush.Color = Color.FromRgb(0, 128, 0);
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = size;
                    rectangle.Height = size;
                    rectangle.Fill = brush;
                    rectangle.Stroke = Brushes.Black;
                    boardCanvas.Children.Add(rectangle);
                    Canvas.SetTop(rectangle, j * size);
                    Canvas.SetLeft(rectangle, i * size);
                    if (gameBoard[i, j] == 0)
                    {
                        brush.Color = Color.FromRgb(255, 255, 255);
                        Ellipse circle = new Ellipse();
                        circle.Width = size;
                        circle.Height = size;
                        circle.Fill = brush;
                        circle.Stroke = Brushes.Black;
                        boardCanvas.Children.Add(circle);
                        Canvas.SetTop(circle, j * size);
                        Canvas.SetLeft(circle, i * size);
                    }
                    if (gameBoard[i, j] == 1)
                    {
                        brush.Color = Color.FromRgb(0, 0, 0);
                        Ellipse circle = new Ellipse();
                        circle.Width = size;
                        circle.Height = size;
                        circle.Fill = brush;
                        circle.Stroke = Brushes.White;
                        boardCanvas.Children.Add(circle);
                        Canvas.SetTop(circle, j * size);
                        Canvas.SetLeft(circle, i * size);
                    }
                }
            }
        }
    }
}
