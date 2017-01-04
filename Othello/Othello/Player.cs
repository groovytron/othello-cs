using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class Player
    {
        private int time;
        private int score;
        private bool isWhite;

        public Player(bool isWhite)
        {
            time = 0;
            score = 0;
            this.IsWhite = isWhite;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\nColor Player : " + (isWhite == true ? "white" : "black"));
            sb.Append($"\nRemaining Time : {time}");
            sb.Append($"\nScore : {score}\n");
            return sb.ToString();
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

        public bool IsWhite
        {
            get
            {
                return isWhite;
            }

            set
            {
                isWhite = value;
            }
        }
    }
}
