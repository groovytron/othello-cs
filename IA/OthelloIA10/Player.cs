using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OthelloIA10
{
    class Player
    {
        private static readonly int GAME_TIME = 15 * 60;
        private int time;
        private int score;
        private Timer timer;
        private GameBoard game;

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
            }
        }

        /// <summary>
        /// Game the player belongs to.
        /// </summary>
        public GameBoard Game
        {
            get; set;
        }
        #endregion

        public Player(GameBoard game)
        {
            this.game = game;
            time = GAME_TIME;
            score = 0;
            Time = GAME_TIME;
            timer = new Timer(1000);
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
            
        }
    }
}
