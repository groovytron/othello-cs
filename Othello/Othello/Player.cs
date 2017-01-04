using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloConsole
{
    class Player
    {
        private int time;
        private int score;

        public Player()
        {
            
        }

        public int Time
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

    }
}
