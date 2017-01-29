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

        #region properties
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
        #endregion

        public Tile(int x, int y, int value, Game game)
        {
            this.x = x;
            this.y = y;
            this.value = value;
            this.game = game;
        }

        /// <summary>
        /// Return the list of the tile's neighbour in the game board.
        /// </summary>
        /// <returns>List of the tile's neighbours</returns>
        public List<Tile> neighbours()
        {
            List<Tile> voisin = new List<Tile>();
            
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var xx = x + dx;
                    var yy = y + dy;
                    if (dx == 0 && dy == 0) { }
                    if (xx >= 0 && xx < 8 && yy >= 0 && yy < 8)
                    {
                        if(game.Board[xx, yy].value != -1)
                        {
                            voisin.Add(game.Board[xx, yy]);
                        }
  
                    }                   
                   
                }
            }
            return voisin;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"\nPoisition : ({x},{y})");
            stringBuilder.Append($"\nValeur : {value}\n");
            return stringBuilder.ToString();
        }
    }
}
