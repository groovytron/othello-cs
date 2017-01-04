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

        private Player[] players;
        private int[,] board;
        private const int BOARDSIZE = 8;
        #region properties
        public int WhiteScore {
            get
            {
                return players[0].Score;
            }
        }
        public int WhiteTime
        {
            get
            {
                return players[0].Time;
            }
        }
        public int BlackScore
        {
            get
            {
                return players[1].Score;
            }
        }
        public int BlackTime
        {
            get
            {
                return players[1].Time;
            }
        }
        #endregion
        public Game()
        {
            players = new Player[2];
            players[0] = new Player(true);
            players[1] = new Player(false);
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
        }

        private void showBoard()
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    Console.Write(board[i,j]);
                }
            }
            Console.WriteLine();
        }

        private void showPlayerStatus()
        {
            for (int j = 0; j < players.Length; j++)
            {
                Console.Write(players[j]);
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
            throw new NotImplementedException();
        }
    }
}
