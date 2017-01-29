using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Othello
{
    class Player : INotifyPropertyChanged
    {
        private static readonly int GAME_TIME = 15 * 60;
        private int time;
        private int score;
        private Timer timer;
        public event PropertyChangedEventHandler PropertyChanged;
        private Game game;

        public Player(Game game)
        {
            this.game = game;
            time = GAME_TIME;
            score = 0;
            Time = GAME_TIME;
            timer = new Timer(1000);
            timer.Elapsed += DecrementTime;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        private void DecrementTime(Object source, ElapsedEventArgs e)
        {
            if (Time - 1 < 0)
            {
                timer.Stop();
                game.GameOver("Le temps d'un joueur est arrivé à terme.");
            }
            Time = Time - 1;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nRemaining Time : {time}");
            sb.Append($"\nScore : {score}\n");
            return sb.ToString();
        }

        public void reset()
        {
            time = GAME_TIME;
            score = 0;
            Time = GAME_TIME;
            timer = new Timer(1000);
            timer.Elapsed += DecrementTime;
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
                raisePropertyChanged("Time");
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

        public Game Game
        {
            get;set;
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
