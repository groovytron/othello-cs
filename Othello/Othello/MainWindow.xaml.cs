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
        public MainWindow()
        {
            InitializeComponent();
            new Game();
            DrawBoard();
        }

        /// <summary>
        /// Draws the board in the window's canvas.
        /// </summary>
        private void DrawBoard()
        {
            int min = (int) Math.Min(board.Width, board.Height);
            SolidColorBrush brush = new SolidColorBrush();

            brush.Color = Color.FromRgb(0, 128, 0);
            board.Width = min;
            board.Height = min;

            int size = min / 8;

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = size;
                    rectangle.Height = size;
                    rectangle.Fill = brush;
                    rectangle.Stroke = Brushes.Black;
                    board.Children.Add(rectangle);
                    System.Console.WriteLine($"Rectangle {i}, {j} placé");
                    Canvas.SetTop(rectangle, j * size);
                    Canvas.SetLeft(rectangle, i * size);
                }
            }
        }
    }
}
