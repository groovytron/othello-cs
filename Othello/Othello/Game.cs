using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OthelloConsole;
using Newtonsoft.Json.Linq;
using System.IO;
using System.ComponentModel;

namespace Othello
{
    class Game : IPlayable, INotifyPropertyChanged
    {
        #region properties
        public bool Paused {
            get
            {
                return paused;
            }
        }

        public Player WhitePlayer
        {
            get
            {
                return players["white"];
            }
        }
        public Player BlackPlayer
        {
            get
            {
                return players["black"];
            }
        }
        public Tile [,] Board
        {
            get { return this.board; }
            set { this.board = value;  }
        }
        public bool CurrentPlayer
        {
            get
            {
                return this.isWhiteTurn;
            }
        }
        public string CurrentPlayerName
        {
            get
            {
                return currentPlayerName;
            }
            set
            {
                currentPlayerName = value;
                raisePropertyChanged("CurrentPlayerName");
            }
        }

        public Player CurrentPlayerInstance
        {
            get
            {
                return isWhiteTurn ? WhitePlayer : BlackPlayer;
            }
        }

        public double RelativeScore
        {
            get { return relativeScore; }
            set { relativeScore = value; raisePropertyChanged("RelativeScore"); }
        }

        internal List<Tile> Playable
        {
            get
            {
                return playable;
            }

            set
            {
                playable = value;
            }
        }
        #endregion
        private Dictionary<string, Player> players;
        private Tile[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        private List<Tile> playable;
        private string currentPlayerName;
        private double relativeScore;
        private int totalScore;
        private bool paused;
        private MainWindow mainWindow;
        private string winner;

        public event PropertyChangedEventHandler PropertyChanged;

        //private int time;
        public Game(MainWindow mainWindow)
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player(this));
            players.Add("black", new Player(this));
            board = new Tile[BOARDSIZE, BOARDSIZE];
            Playable = new List<Tile>();
            relativeScore = 0;
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[j, i] = new Tile(j, i, -1, this);
                }
            }
            //newGame();
            this.winner = "";
            this.mainWindow = mainWindow;
        }

        private void showBoard()
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    Console.Write($"\t{board[i,j].Value}");
                }
            }
            Console.WriteLine();
        }

        private void showPlayerStatus()
        {
            foreach (var player in players)
            {
                Console.WriteLine(player.Value);
            }
        }

        public void save(string filename)
        {

            JObject playersObject = new JObject();

            foreach (var player in players)
            {
                JObject o = new JObject();
                o.Add("score",player.Value.Score);
                o.Add("time", player.Value.Time);
                playersObject.Add(player.Key,o);
            }

            JArray boardObject = new JArray();

            foreach (var tile in board)
            {
                JObject o = new JObject();
                o.Add("X", tile.X);
                o.Add("Y", tile.Y);
                o.Add("Value", tile.Value);
                boardObject.Add(o);
            }

            JObject gameObject = new JObject();
            gameObject.Add("Players",playersObject);
            gameObject.Add("Board",boardObject);
            gameObject.Add("isWhiteTurn", isWhiteTurn);

            File.WriteAllText(filename, gameObject.ToString());
        }

        public void load(string filename)
        {
            JObject gameObject = JObject.Parse(File.ReadAllText(filename));

            var playersObject = (JObject)gameObject["Players"];

            players["white"].Score = (int)playersObject["white"]["score"];
            players["white"].Time = (int)playersObject["white"]["time"];
            players["white"].Game = this;

            players["black"].Score = (int)playersObject["black"]["score"];
            players["black"].Time = (int)playersObject["black"]["time"];
            players["black"].Game = this;

            isWhiteTurn = (bool) gameObject["isWhiteTurn"];

            foreach (var tile in gameObject["Board"])
            {
                int x = (int)tile["X"];
                int y = (int)tile["Y"];
                int value = (int)tile["Value"];
                Board[x,y].Value  = value;
            }
            getPlayableTile(isWhiteTurn);
        }

        public void newGame()
        {
            foreach (var player in players)
            {
                player.Value.reset();
            }
            foreach (var tile in board)
            {
                board[tile.X, tile.Y].Value = -1;
            }

            board[3, 3].Value = 0;
            board[4, 3].Value = 1;
            board[3, 4].Value = 1;
            board[4, 4].Value = 0;
            
            updateScore();

            isWhiteTurn = false;
            currentPlayerName = isWhiteTurn ? "White" : "Black";

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;
            //BlackPlayer.reset();
            //WhitePlayer.reset();
            showBoard();
            getPlayableTile(isWhiteTurn);
            CurrentPlayerInstance.StartTimer();
            paused = false;
            winner = "";
        }

        public void Pause()
        {
            paused = !paused;
            if (paused)
            {
                CurrentPlayerInstance.StopTimer();
            }
            else
            {
                CurrentPlayerInstance.StartTimer();
            }
            raisePropertyChanged("Paused");
        }

        internal void GameOver(string message)
        {
            Pause();
            if(WhitePlayer.Time == 0 || BlackPlayer.Time == 0)
            {
                winner = BlackPlayer.Time <= 0 ? "White" : "Black";
            }
            else
            {
                winner = BlackPlayer.Score > WhitePlayer.Score ? "Black" : "White";
            }
            
            mainWindow.GameOver(message);
        }

        public string getWinner()
        {
            return winner;
        }

        public void getPlayableTile(bool isWhiteTurn)
        {
            List<Tile> potential = new List<Tile>();
            HashSet<Tile> playableSet = new HashSet<Tile>();
            int enemy = isWhiteTurn ? 0 : 1 ;
            int me = isWhiteTurn ? 1 : 0;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j].Value == -1 && board[i, j].voisin().Count > 0)
                    {
                        potential.Add(board[i, j]);
                    }
                }
            }

            foreach (var tile in potential)
            {
                foreach (var neighbor in tile.voisin())
                {
                    if(neighbor.Value == enemy)
                    {
                        if (checkTile(new Tile(tile.X, tile.Y, me, this), neighbor))
                        {
                            playableSet.Add(tile);
                            break;
                        }
                    }
                }
            }
            Playable = playableSet.ToList<Tile>();
            //return playable;
        }
        
        private bool checkTile(Tile tile, Tile neighbor)
        {
            int offsetX = tile.X - neighbor.X;
            int offsetY = tile.Y - neighbor.Y;
            Tile visited;
            int x;
            int y;
            do
            {
                
                x = neighbor.X - offsetX;
                y = neighbor.Y - offsetY;
                if (x < 0 || y < 0 || x > 7 || y > 7)
                {
                    return false;
                }
                visited = board[x, y];
                if (visited.Value == tile.Value)
                {
                    return true;
                }
                neighbor = visited;
            } while (visited.Value != tile.Value && visited.Value != -1);

            return false;
        }

        private void flippeTiles(Tile tile, bool isWhiteTurn)
        {
            Console.WriteLine("flipe");
            int enemy = isWhiteTurn ? 0 : 1;
            int me = isWhiteTurn ? 1 : 0;

            foreach (var neighbor in tile.voisin())
            {
                List<Tile> hasToBeFlipped = new List<Tile>();
                Tile neighborTile = neighbor;
                if (neighbor.Value == enemy)
                {
                    bool flip = false;                            
                    int offsetX = tile.X - neighbor.X;
                    int offsetY = tile.Y - neighbor.Y;
                    int x = neighborTile.X;
                    int y = neighborTile.Y;
                    if (x < 0 && x > 7 && y < 0 && y > 7)
                    {
                        return;
                    }
                    Tile visited = board[x, y];
                    while (visited.Value != -1 && x >= 0 && x < 8 && y >= 0 && y < 8)
                    {
                        visited = board[x, y];
  
                        if (visited.Value == enemy)
                        {
                            hasToBeFlipped.Add(neighborTile);
                        }
                        else if (visited.Value == me)
                        {
                            flip = true;
                            break;
                        }
                        x = neighborTile.X - offsetX;
                        y = neighborTile.Y - offsetY;
                        
                        neighborTile = visited;
                    }

                    if (flip)
                    {
                        foreach (var tileToFlip in hasToBeFlipped)
                        {
                            board[tileToFlip.X, tileToFlip.Y].Value = me;
                        }
                    }
                }
            }
        }

        public int getBlackScore()
        {
            return players["black"].Score;
        }

        public int getWhiteScore()
        {
            return players["white"].Score;
        }

        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public bool isPlayable(int column, int line, bool isWhite)
        {
            int color = isWhite ? 1 : 0;
            foreach (var tile in Playable)
            {
                if(tile.X == line && tile.Y == column){
                    return true;
                }
            }
            return false;
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            if (Paused)
            {
                return false;
            }
            getPlayableTile(isWhite);
            if (!isPlayable(column, line, isWhite))
            {
                return false;
            }
            CurrentPlayerInstance.StopTimer();

            board[line, column].Value = isWhite ? 1 : 0;
            flippeTiles(board[line, column], isWhite);
            isWhiteTurn = !isWhiteTurn;
        
            updateProperties();
            getPlayableTile(isWhiteTurn);
            if (this.Playable.Count == 0)
            {
                isWhiteTurn = !isWhiteTurn;
                updateProperties();
            }
            CurrentPlayerInstance.StartTimer();
            getPlayableTile(isWhiteTurn);
            updateScore();
            showBoard();
            if(totalScore == 64)
            {
                //fin du jeux 
                GameOver("All the pawns have been placed.");
            }
            return true;
        }

        private void updateScore()
        {
            int b = 0; 
            int w= 0;
            foreach (var tile in board)
            {
                if(tile.Value == 1)
                {
                    w++;
                }else if (tile.Value == 0)
                {
                    b++;
                }
            }
            totalScore = b + w;
            players["white"].Score = w;
            players["black"].Score = b;
            relativeScore = (double) b / (double)(w + b);
            raisePropertyChanged("RelativeScore");
        }

        private void updateProperties()
        {
            CurrentPlayerName = isWhiteTurn ? "White" : "Black";
            raisePropertyChanged("CurrentPlayerName");
        }

        private void raisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
