using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class Tile
    {

        private int x;
        private int y;
        private int value;
        private Game game;

        public int Value 
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public Tile(int x, int y, int value, Game game)
        {
            this.x = x;
            this.y = y;
            this.value = value;
            this.game = game;
        }

        public List<Tile> voisin()
        {
            List<Tile> voisin = new List<Tile>();
            
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) { }
                    if (x + dx >= 0 && x + dx < 8 && y + dy >= 0 && y + dy < 8)
                    {
                        if(game.Board[x + dx, y + dy].value != -1)
                        {
                            voisin.Add(game.Board[x + dx, y + dy]);
                        }
  
                    }                   
                   
                }
            }
            return voisin;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nPoisition : ({x},{y})");
            sb.Append($"\nValeur : {value}\n");
            return sb.ToString();
        }

    }
}
