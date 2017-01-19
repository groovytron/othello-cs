using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OthelloConsole;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Othello
{
    class Game : IPlayable
    {
        #region properties
        public int WhiteScore
        {
            get
            {
                return players["white"].Score;
            }
        }
        public int WhiteTime
        {
            get
            {
                return players["white"].Time;
            }
        }
        public int BlackScore
        {
            get
            {
                return players["black"].Score;
            }
        }
        public int BlackTime
        {
            get
            {
                return players["black"].Time;
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
                return isWhiteTurn ? "White" : "Black";
            }
        }
        public double BlackPawns
        {
            get { return BlackScore / placedPawnsCount; }
            set { this.BlackPawns = value; }
        }
        #endregion
        private Dictionary<string, Player> players;
        private Tile[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        private int placedPawnsCount;
        private List<Tile> playable;

        //private int time;
        public Game()
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player());
            players.Add("black", new Player());
            board = new Tile[BOARDSIZE, BOARDSIZE];
            playable = new List<Tile>();
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i, j] = new Tile(i, j, -1, this);
                }
            }
            newGame();
            placedPawnsCount = 4;
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

        public void save()
        {
            JObject o = new JObject
            {
                {
                    "Players" , new JObject
                    {
                        {
                            "white", new JObject
                            {
                                {"score", players["white"].Score },
                                {"time" , players["white"].Time }
                            }
                        }
                            ,
                        {

                            "black", new JObject
                            {
                                {"score", players["black"].Score },
                                {"time" , players["black"].Time }
                            }
                        }
                    }
                    },
                {
                    "Board" , new JArray(board)
                }
            };

            File.WriteAllText("partie.json", o.ToString());
        }

        public void load()
        {
            JObject o = JObject.Parse(File.ReadAllText("partie.json"));

            var a = (JObject)o["Players"];

            players["white"].Score = (int)a["white"]["score"];
            players["white"].Time = (int)a["white"]["time"];

            players["black"].Score = (int)a["black"]["score"];
            players["black"].Time = (int)a["black"]["time"];


            var b = o["Board"].ToObject<int[]>();

            var h = 0;
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    //board[i, j] = b[h++];
                }
            }

        }

        public void newGame()
        {
            foreach (var player in players)
            {
                player.Value.reset();
            }

            board[3, 3].Value = 0;
            board[3, 4].Value = 1;
            board[4, 3].Value = 1;
            board[4, 4].Value = 0;

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
                foreach ( var neighbor in tile.voisin())
                {
                    if(neighbor.Value == enemy)
                    {
                        if (checkTile(new Tile(tile.X,tile.Y, me, this), neighbor))
                        {
                            playableSet.Add(tile);
                            break;
                        }
                    }
                }
            }
            playable = playableSet.ToList<Tile>();
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
                if (neighbor.X == 0 || neighbor.Y == 0 || neighbor.X == 7 || neighbor.Y == 7)
                {
                    return false;
                }
                visited = board[x, y];
                if (visited.Value == tile.Value)
                {
                    return true;
                }
                neighbor = visited;
            } while (visited.Value != tile.Value && visited.Value != -1 && x > 1 && x < 7 && y > 1 && y < 7);

            return false;
        }

        private void flippeTiles(Tile tile, bool isWhiteTurn)
        {
            int enemy = isWhiteTurn ? 0 : 1;
            int me = isWhiteTurn ? 1 : 0;
            foreach (var neighbor in tile.voisin())
            {
                Tile neighborTile = neighbor;
                if (neighbor.Value == enemy)
                {
                    int offsetX = tile.X - neighbor.X;
                    int offsetY = tile.Y - neighbor.Y;
                    Tile visited;
                    int x;
                    int y;
                    do
                    {

                        x = neighborTile.X - offsetX;
                        y = neighborTile.Y - offsetY;
                        if (neighbor.X == 0 || neighbor.Y == 0 || neighbor.X == 7 || neighbor.Y == 7)
                        {
                            break;
                        }
                        visited = board[x, y];
                        if (visited.Value == tile.Value)
                        {
                            Board[visited.X, visited.Y].Value = me;
                        }
                        neighborTile = visited;
                    } while (visited.Value != tile.Value && visited.Value != -1 && x > 1 && x < 7 && y > 1 && y < 7);
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
                if(tile.X == column && tile.Y == line){
                    return true;
                }
            }
            return false;
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            getPlayableTile(isWhite);
            //Console.WriteLine("Before playing");
            //foreach (var tile in this.playable)
            //{
            //    Console.WriteLine($"Playable at x:{tile.X}, y:{tile.Y}");
            //}
            if (!isPlayable(column, line, isWhite))
            {
                return false;
            }

            board[line, column].Value = isWhite ? 1 : 0;
            flippeTiles(board[line, column], isWhite);
            isWhiteTurn = !isWhiteTurn;
            getPlayableTile(isWhiteTurn);
            if (this.playable.Count == 0)
            {
                isWhiteTurn = !isWhiteTurn;
            }
            getPlayableTile(!isWhite);
            //Console.WriteLine("After playing");
            //foreach (var tile in this.playable)
            //{
            //    Console.WriteLine($"Playable at x:{tile.X}, y:{tile.Y}");
            //}
            showBoard();
            return true;
        }
    }
}
