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
        /// <summary>
        /// Game pause state. Game is paused if this value is true.
        /// </summary>
        public bool Paused {
            get
            {
                return paused;
            }
        }

        /// <summary>
        /// White player instance.
        /// </summary>
        public Player WhitePlayer
        {
            get
            {
                return players["white"];
            }
        }

        /// <summary>
        /// Black player instance.
        /// </summary>
        public Player BlackPlayer
        {
            get
            {
                return players["black"];
            }
        }

        /// <summary>
        /// 2D array containing the board state (free tiles, black tiles and white tiles).
        /// </summary>
        public Tile [,] Board
        {
            get { return this.board; }
            set { this.board = value;  }
        }

        /// <summary>
        /// Is true if it is white player's turn. Is false if it is black player's.
        /// </summary>
        public bool CurrentPlayer
        {
            get
            {
                return this.isWhiteTurn;
            }
        }

        /// <summary>
        /// Current player name; "White" ou "Black"
        /// </summary>
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

        /// <summary>
        /// Playing player instance.
        /// </summary>
        public Player CurrentPlayerInstance
        {
            get
            {
                return isWhiteTurn ? WhitePlayer : BlackPlayer;
            }
        }

        /// <summary>
        /// Relative score of black player compared to the total pawns placed.
        /// </summary>
        public double RelativeScore
        {
            get { return relativeScore; }
            set { relativeScore = value; raisePropertyChanged("RelativeScore"); }
        }

        /// <summary>
        /// Game's interface for AI integration.
        /// </summary>
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
        private Stack<Tile[,]> boardStack;

        public event PropertyChangedEventHandler PropertyChanged;

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
            boardStack = new Stack<Tile[,]>();
        }

        #region debugging methods
        /// <summary>
        /// Print the actual board's state in the console for debugging.
        /// </summary>
        private void ShowBoard()
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

        /// <summary>
        /// Print players' values in the console for debugging.
        /// </summary>
        private void ShowPlayerStatus()
        {
            foreach (var player in players)
            {
                Console.WriteLine(player.Value);
            }
        }
        #endregion

        /// <summary>
        /// Save the actual game state (board, turn, player's scores and time)
        /// in the specified file. Datas are save in JSON format.
        /// </summary>
        /// <param name="filename">File where the game has to be saved.</param>
        public void Save(string filename)
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

        /// <summary>
        /// Load a game from a JSON file.
        /// </summary>
        /// <param name="filename">File containing the datas to be loaded.</param>
        public void Load(string filename)
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
            GetPlayableTile(isWhiteTurn);
        }

        /// <summary>
        /// Initialise players and board for a new game.
        /// </summary>
        public void NewGame()
        {
            boardStack.Clear();
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
            ShowBoard();
            GetPlayableTile(isWhiteTurn);
            CurrentPlayerInstance.StartTimer();
            paused = false;
            winner = "";

            //boardStack.Push((Tile[,])Board.Clone());
            boardStack.Push(CopyBoard());
        }

        /// <summary>
        /// Used for the board deepcopy and undo methods.
        /// </summary>
        /// <returns></returns>
        private Tile[,] CopyBoard()
        {
            Tile[,] copy = new Tile[BOARDSIZE, BOARDSIZE];
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    copy[i, j] = new Tile(Board[i, j].X, Board[i, j].Y, Board[i, j].Value, this);
                }
            }
            
            return copy;
        }

        /// <summary>
        /// Stops the timers and disable the possibily to place pawns.
        /// If game was paused it is resumed and timers are restarted.
        /// </summary>
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

        /// <summary>
        /// Undo the last move. (Not working)
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            try
            {
                Console.WriteLine("Undo");
                Board = boardStack.Pop();
                Board = boardStack.Pop();
                //Tile[,] popped = boardStack.Pop();
                //for (int i = 0; i < BOARDSIZE; i++)
                //{
                //    Console.WriteLine();
                //    for (int j = 0; j < BOARDSIZE; j++)
                //    {
                //        Console.Write($"\t{board[i, j].Value}");
                //    }
                //    Console.WriteLine();
                //}
                isWhiteTurn = !isWhiteTurn;
                updateProperties();
                ShowBoard();
            }
            catch (InvalidOperationException e)
            {
                return false;
            }
            return true;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Stop the game and pass the game over message containing the game over reason to the UI.
        /// </summary>
        /// <param name="message">Message containing the gameover reason.</param>
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

        /// <summary>
        /// Return the winner's name.
        /// </summary>
        /// <returns></returns>
        public string GetWinner()
        {
            return winner;
        }

        /// <summary>
        /// Return the playable tiles for the player that has to play.
        /// </summary>
        /// <param name="isWhiteTurn"></param>
        public void GetPlayableTile(bool isWhiteTurn)
        {
            List<Tile> potential = new List<Tile>();
            HashSet<Tile> playableSet = new HashSet<Tile>();
            int enemy = isWhiteTurn ? 0 : 1 ;
            int me = isWhiteTurn ? 1 : 0;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j].Value == -1 && board[i, j].neighbours().Count > 0)
                    {
                        potential.Add(board[i, j]);
                    }
                }
            }

            foreach (var tile in potential)
            {
                foreach (var neighbor in tile.neighbours())
                {
                    if(neighbor.Value == enemy)
                    {
                        if (CheckTile(new Tile(tile.X, tile.Y, me, this), neighbor))
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
        
        /// <summary>
        /// Checks if the tile is playable with a given research direction.
        /// </summary>
        /// <param name="tile">Tile that has to be checked.</param>
        /// <param name="neighbor">Neighbour giving the direction of the research.</param>
        /// <returns></returns>
        private bool CheckTile(Tile tile, Tile neighbor)
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

        /// <summary>
        /// Flip the tiles following the tile that has been placed.
        /// </summary>
        /// <param name="tile">Tile that has been placed.</param>
        /// <param name="isWhiteTurn">True: player who placed the pawn is white. False: player is black.</param>
        private void FlipTiles(Tile tile, bool isWhiteTurn)
        {
            Console.WriteLine("flipe");
            int enemy = isWhiteTurn ? 0 : 1;
            int me = isWhiteTurn ? 1 : 0;

            foreach (var neighbor in tile.neighbours())
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

        /// <summary>
        /// Return black player score.
        /// </summary>
        /// <returns></returns>
        public int getBlackScore()
        {
            return players["black"].Score;
        }

        /// <summary>
        /// Return white player score.
        /// </summary>
        /// <returns></returns>
        public int getWhiteScore()
        {
            return players["white"].Score;
        }

        /// <summary>
        /// Not implemented yet. This method is for AI integration.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return true if the move is playable or not.
        /// </summary>
        /// <param name="column">Move's X position</param>
        /// <param name="line">Move's Y position</param>
        /// <param name="isWhite">Player who's trying to place the tile</param>
        /// <returns></returns>
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

        /// <summary>
        /// Play the move if it is playable and valid.
        /// </summary>
        /// <param name="column">Move's X position</param>
        /// <param name="line">Move's Y position</param>
        /// <param name="isWhite">Player who's trying to place the tile</param>
        /// <returns></returns>
        public bool playMove(int column, int line, bool isWhite)
        {
            if (Paused)
            {
                return false;
            }
            GetPlayableTile(isWhite);
            if (!isPlayable(column, line, isWhite))
            {
                return false;
            }
            CurrentPlayerInstance.StopTimer();

            board[line, column].Value = isWhite ? 1 : 0;
            FlipTiles(board[line, column], isWhite);
            isWhiteTurn = !isWhiteTurn;
        
            updateProperties();
            GetPlayableTile(isWhiteTurn);
            if (this.Playable.Count == 0)
            {
                isWhiteTurn = !isWhiteTurn;
                updateProperties();
            }
            CurrentPlayerInstance.StartTimer();
            GetPlayableTile(isWhiteTurn);
            updateScore();
            ShowBoard();
            if(totalScore == 64)
            {
                //fin du jeux 
                GameOver("All the pawns have been placed.");
            }
            boardStack.Push(CopyBoard());
            //boardStack.Push((Tile[,])Board.Clone());
            return true;
        }

        /// <summary>
        /// Update players' scores.
        /// </summary>
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

        /// <summary>
        /// Update properties and raise event for objects doing data binding
        /// on the game.
        /// </summary>
        private void updateProperties()
        {
            CurrentPlayerName = isWhiteTurn ? "White" : "Black";
            raisePropertyChanged("CurrentPlayerName");
        }

        /// <summary>
        /// Dipatch property update for objects doing data binding on the game.
        /// </summary>
        /// <param name="propertyName"></param>
        private void raisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
