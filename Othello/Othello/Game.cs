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

        public double RelativeScore
        {
            get { return relativeScore; }
            set { relativeScore = value; raisePropertyChanged("RelativeScore"); }
        }
        #endregion
        private Dictionary<string, Player> players;
        private Tile[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        private List<Tile> playable;
        private string currentPlayerName;
        private double relativeScore;

        public event PropertyChangedEventHandler PropertyChanged;

        //private int time;
        public Game()
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player());
            players.Add("black", new Player());
            board = new Tile[BOARDSIZE, BOARDSIZE];
            playable = new List<Tile>();
            relativeScore = 0;
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[j, i] = new Tile(j, i, -1, this);
                }
            }
            newGame();
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

            File.WriteAllText(filename, gameObject.ToString());
        }

        public void load(string filename)
        {
            JObject gameObject = JObject.Parse(File.ReadAllText(filename));

            var playersObject = (JObject)gameObject["Players"];

            players["white"].Score = (int)playersObject["white"]["score"];
            players["white"].Time = (int)playersObject["white"]["time"];

            players["black"].Score = (int)playersObject["black"]["score"];
            players["black"].Time = (int)playersObject["black"]["time"];


            foreach (var tile in gameObject["Board"])
            {
                int x = (int)tile["X"];
                int y = (int)tile["Y"];
                int value = (int)tile["Value"];
                Board[x,y].Value  = value;
            }
        }

        public void newGame()
        {
            foreach (var player in players)
            {
                player.Value.reset();
            }

            board[3, 3].Value = 0;
            board[4, 3].Value = 1;
            board[3, 4].Value = 1;
            board[4, 4].Value = 0;
            updateScore();

            /*board[0, 3].Value = 0;
            board[0, 4].Value = 1;
            board[0, 5].Value = 0;
            board[0, 7].Value = 0;

            board[1, 2].Value = 0;
            board[1, 3].Value = 1;
            board[1, 4].Value = 1;
            board[1, 6].Value = 0;

            board[2, 1].Value = 0;
            board[2, 2].Value = 1;
            board[2, 3].Value = 1;
            board[2, 4].Value = 1;
            board[2, 5].Value = 1;

            board[3, 0].Value = 0;
            board[3, 2].Value = 0;
            board[3, 3].Value = 0;
            board[3, 4].Value = 1;
            board[3, 5].Value = 0;
            board[3, 6].Value = 0;
            board[3, 7].Value = 0;

            board[4, 1].Value = 0;
            board[4, 2].Value = 1;
            board[4, 3].Value = 0;
            board[4, 4].Value = 1;
            board[4, 5].Value = 1;
            board[4, 6].Value = 0;

            board[5, 0].Value = 0;
            board[5, 1].Value = 0;
            board[5, 2].Value = 0;
            board[5, 3].Value = 1;
            board[5, 4].Value = 1;
            board[5, 5].Value = 0;

            board[6, 1].Value = 0;
            board[6, 4].Value = 0;

            board[7, 0].Value = 0;
            board[7, 3].Value = 0;*/



            isWhiteTurn = false;
            currentPlayerName = isWhiteTurn ? "White" : "Black";

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;
            ///

            showBoard();
            getPlayableTile(true);
        }

        public void pause()
        {

        }

        public List<Tile> getPlayableTile(bool isWhiteTurn)
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
            playable = playableSet.ToList<Tile>();
            return playable;
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

                    /*int offsetX = tile.X - neighborTile.X;
                    int offsetY = tile.Y - neighborTile.Y;
                    Tile visited;
                    int x;
                    int y;
                    do
                    {

                        x = neighborTile.X - offsetX;
                        y = neighborTile.Y - offsetY;
                        if (x < 0 || y < 0 || x > 7 || y > 7)
                        {
                            break;
                        }
                        visited = board[x, y];
                        if (visited.Value == enemy)
                        {
                            hasToBeFlipped.Add(neighborTile);

                        }
                        if (visited.Value == me)
                        {
                            flip = true;
                            break;
                        }
                        neighborTile = visited;
                    } while (visited.Value != -1);*/

                            
                    int offsetX = tile.X - neighbor.X;
                    int offsetY = tile.Y - neighbor.Y;
                    int x = neighborTile.X;
                    int y = neighborTile.Y;
                    if (x < 0 && x > 7 && y < 0 && y > 7)
                    {
                        return;
                    }
                    Tile visited = board[x, y];
                    while (visited.Value != -1 && x > 0 && x < 8 && y > 0 && y < 8)
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
            foreach (var tile in playable)
            {
                if(tile.X == line && tile.Y == column){
                    return true;
                }
            }
            return false;
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            getPlayableTile(isWhite);
            if (!isPlayable(column, line, isWhite))
            {
                return false;
            }

            board[line, column].Value = isWhite ? 1 : 0;
            flippeTiles(board[line, column], isWhite);
            isWhiteTurn = !isWhiteTurn;
            
            updateProperties();
            getPlayableTile(isWhiteTurn);
            if (this.playable.Count == 0)
            {
                isWhiteTurn = !isWhiteTurn;
                updateProperties();
            }
            getPlayableTile(!isWhite);
            updateScore();
            showBoard();
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
