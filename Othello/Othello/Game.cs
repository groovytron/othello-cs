using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OthelloConsole;

namespace Othello
{
    class Game : IPlayable
    {
        #region properties
        public int WhiteScore
        {
            get
            {
                return players["white"].Score;
            }
        }
        public int WhiteTime
        {
            get
            {
                return players["white"].Time;
            }
        }
        public int BlackScore
        {
            get
            {
                return players["black"].Score;
            }
        }
        public int BlackTime
        {
            get
            {
                return players["black"].Time;
            }
        }
        public int [,] Board
        {
            get { return this.board; }
        }
        public string CurrentPlayerName
        {
            get
            {
                if (isWhiteTurn)
                {
                    return "White";
                }
                else
                {
                    return "Black";
                }
            }
        }
        #endregion
        private Dictionary<string, Player> players;
        private int[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        //private int time;
        public Game()
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player());
            players.Add("black", new Player());
            board = new int[BOARDSIZE, BOARDSIZE];
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i,j] = -1;
                }
            }
            showBoard();
            showPlayerStatus();

            newGame();
        }

        private void showBoard()
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    Console.Write($"\t{board[i,j]}");
                }
            }
            Console.WriteLine();
        }

        private void showPlayerStatus()
        {
            foreach (var player in players)
            {
                Console.WriteLine(player.Value);
            }
        }

        public void save()
        {

        }

        public void load()
        {

        }

        public void newGame()
        {
            foreach (var player in players)
            {
                player.Value.reset();
            }

            playMove(3, 3, true);
            playMove(4, 4, true);
            playMove(4, 3, false);
            playMove(3, 4, false);

            isWhiteTurn = false;

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;

            showBoard();
            showPlayerStatus();

        }

        public void pause()
        {

        }

        public int getBlackScore()
        {
            throw new NotImplementedException();
        }

        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int getWhiteScore()
        {
            throw new NotImplementedException();
        }

        public bool isPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            if (isWhite)
            {
                board[line, column] = 0;
            }
            else
            {
                board[line, column] = 1;
            }
            return true;
        }
    }
}
