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

        #region properties

        /// <summary>
        /// Player's remaining thinking time in seconds.
        /// </summary>
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

        /// <summary>
        /// Player's score (pawns owned on the game board).
        /// </summary>
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

        /// <summary>
        /// Game the player belongs to.
        /// </summary>
        public Game Game
        {
            get; set;
        }
        #endregion

        public Player(Game game)
        {
            this.game = game;
            time = GAME_TIME;
            score = 0;
            Time = GAME_TIME;
            timer = new Timer(1000);
            timer.Elapsed += DecrementTime;
        }

        /// <summary>
        /// Start the player's timer count down.
        /// </summary>
        public void StartTimer()
        {
            timer.Start();
        }

        /// <summary>
        /// Stop the plaer's timer count down.
        /// </summary>
        public void StopTimer()
        {
            timer.Stop();
        }

        /// <summary>
        /// Decrement player's count down. Called at every timer tick.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DecrementTime(Object source, ElapsedEventArgs e)
        {
            if (Time - 1 < 0)
            {
                timer.Stop();
                game.GameOver("A player's time has expired.");
            }
            Time--;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"\nRemaining Time : {time}");
            sb.Append($"\nScore : {score}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Put all the player's attributes back to
        /// initial values for a new game.
        /// </summary>
        public void reset()
        {
            time = GAME_TIME;
            score = 0;
            Time = GAME_TIME;
            timer = new Timer(1000);
            timer.Elapsed += DecrementTime;
        }

        /// <summary>
        /// Used to dispatch an update event to objects doing
        /// databinding on the player.
        /// </summary>
        /// <param name="v"></param>
        private void raisePropertyChanged(string v)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
    }
}
