using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloConsole
{
    class Game : IPlayable
    {

        private Player[] players;
        private int[,] board;


        public Game()
        {

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
