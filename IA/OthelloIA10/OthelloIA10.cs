using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;

namespace OthelloIA10
{
    public enum EtatCase
    {
        Empty = -1,
        White = 0,
        Black = 1
    }

    class GameBoard : IPlayable.IPlayable
    {
        #region properties
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
        public Tile[,] Board
        {
            get { return this.board; }
            set { this.board = value; }
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
            }
        }

        /// <summary>
        /// Relative score of black player compared to the total pawns placed.
        /// </summary>
        public double RelativeScore
        {
            get { return relativeScore; }
            set { relativeScore = value; }
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
        private List<Tile> playable;
        private string currentPlayerName;
        private double relativeScore;

        public GameBoard()
        {
            players = new Dictionary<string, Player>();
            players.Add("white", new Player(this));
            players.Add("black", new Player(this));
            board = new Tile[BOARDSIZE, BOARDSIZE];
            Playable = new List<Tile>();
            relativeScore = 0;
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[j, i] = new Tile(j, i, (int)EtatCase.Empty, this);
                }
            }
            NewGame();
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
                    Console.Write($"\t{board[i, j].Value}");
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
        /// Initialise players and board for a new game.
        /// </summary>
        public void NewGame()
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

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;
            //BlackPlayer.reset();
            //WhitePlayer.reset();
            ShowBoard();
            //GetPlayableTile(isWhiteTurn);
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
        /// Return the playable tiles for the player that has to play.
        /// </summary>
        /// <param name="isWhiteTurn"></param>
        public void GetPlayableTile(bool isWhiteTurn)
        {
            List<Tile> potential = new List<Tile>();
            HashSet<Tile> playableSet = new HashSet<Tile>();
            int enemy = isWhiteTurn ? 0 : 1;
            int me = isWhiteTurn ? 1 : 0;

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    if (board[i, j].Value == (int)EtatCase.Empty && board[i, j].neighbours().Count > 0)
                    {
                        potential.Add(board[i, j]);
                    }
                }
            }

            foreach (var tile in potential)
            {
                foreach (var neighbor in tile.neighbours())
                {
                    if (neighbor.Value == enemy)
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
            } while (visited.Value != tile.Value && visited.Value != (int)EtatCase.Empty);

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
            int enemy = isWhiteTurn ? (int)EtatCase.White : (int) EtatCase.Black;
            int me = isWhiteTurn ? (int)EtatCase.White : (int)EtatCase.Black;

            ShowBoard();
            Console.ReadLine();

            foreach (var neighbor in tile.neighbours())
            {
                List<Tile> hasToBeFlipped = new List<Tile>();
                Tile neighborTile = neighbor;
                if (neighbor.Value == enemy)
                {
                    bool flip = false;
                    int offsetX = tile.X - neighbor.X;
                    int offsetY = tile.Y - neighbor.Y;

                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    int x = neighborTile.X;
                    int y = neighborTile.Y;
                    if (x < 0 && x > 7 && y < 0 && y > 7)
                    {
                        return;
                    }
                    Tile visited = board[x, y];
                    Console.WriteLine($"offsetX: {offsetX}");
                    Console.WriteLine($"offsetY: {offsetY}");

                    while (visited.Value != -1 && x >= 0 && x < 8 && y >= 0 && y < 8)
                    {
                        Console.WriteLine("While flip");
                        Console.WriteLine($"visited: {visited.Value}");
                        Console.WriteLine($"visited x: {visited.X}");
                        Console.WriteLine($"visited y: {visited.Y}");
                        Console.ReadLine();

                        visited = board[x, y];

                        if (visited.Value == enemy)
                        {
                            Console.WriteLine($"{neighborTile.X}, {neighborTile.Y} needs to be flipped");
                            hasToBeFlipped.Add(neighborTile);
                        }
                        else if (visited.Value == me)
                        {
                            Console.WriteLine("break");
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
        /// Return true if the move is playable or not.
        /// </summary>
        /// <param name="column">Move's X position</param>
        /// <param name="line">Move's Y position</param>
        /// <param name="isWhite">Player who's trying to place the tile</param>
        /// <returns></returns>
        public bool isPlayable(int column, int line, bool isWhite)
        {
            int color = isWhite ? (int)EtatCase.White : (int)EtatCase.Black;
            foreach (var tile in Playable)
            {
                if (tile.X == line && tile.Y == column)
                {
                    return true;
                }
            }
            return false;
        }


        string IPlayable.IPlayable.GetName()
        {
            return "Droxler M'Poy";
        }

        bool IPlayable.IPlayable.IsPlayable(int column, int line, bool isWhite)
        {
            int color = isWhite ? (int)EtatCase.White : (int)EtatCase.Black;
            foreach (var tile in Playable)
            {
                if (tile.X == line && tile.Y == column)
                {
                    return true;
                }
            }
            return false;
        }

        bool IPlayable.IPlayable.PlayMove(int column, int line, bool isWhite)
        {
            //GetPlayableTile(isWhite);
            //if (!isPlayable(column, line, isWhite))
            //{
            //    Console.WriteLine($"{(isWhite ? "WHITE" : "BLACK")} joue.");
            //    Console.WriteLine("Coup injouable");
            //    Console.ReadLine();
            //    return false;
            //}

            board[line, column].Value = isWhite ? (int)EtatCase.White : (int)EtatCase.Black;
            FlipTiles(board[line, column], isWhite);
           
            ShowBoard();

            return true;
        }

        Tuple<int, int> IPlayable.IPlayable.GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            Random rnd = new Random();
            GetPlayableTile(whiteTurn);
            if (Playable.Count <= 0)
            {
                return new Tuple<int, int>(-1, -1);
            }
            else
            {
                Tile choice = Playable[rnd.Next(Playable.Count)];
                Console.WriteLine($"I am {(whiteTurn ? "WHITE" : "BLACK")}");
                Console.WriteLine($"Played at ({choice.X}; {choice.Y})");
                Console.ReadLine();
                return new Tuple<int, int>(choice.X, choice.Y);
            }
        }

        int[,] IPlayable.IPlayable.GetBoard()
        {
            int[,] intBoard = new int[BOARDSIZE, BOARDSIZE];

            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    intBoard[i, j] = board[i, j].Value;
                }
            }
            return intBoard;
        }

        int IPlayable.IPlayable.GetWhiteScore()
        {
            return WhitePlayer.Score;
        }

        int IPlayable.IPlayable.GetBlackScore()
        {
            return BlackPlayer.Score;
        }
    }
}
