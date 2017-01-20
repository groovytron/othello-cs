using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class Player : INotifyPropertyChanged
    {
        private int time;
        private int score;

        public event PropertyChangedEventHandler PropertyChanged;

        public Player()
        {
            time = 0;
            score = 0;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nRemaining Time : {time}");
            sb.Append($"\nScore : {score}\n");
            return sb.ToString();
        }

        public void reset()
        {
            time = 0;
            score = 0;
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
                raisePropertyChanged("Score");
            }
        }

        private void raisePropertyChanged(string v)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
    }
}
