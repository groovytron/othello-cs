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


        public Game()
        {
            players = new Player[2];
            players[0] = new Player(true);
            players[1] = new Player(false);
            board = new int[8, 8];
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board.Length; j++)
                {
                    board[i,j] = -1;
                }
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
