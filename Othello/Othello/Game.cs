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
        public string CurrentPlayerName
        {
            get
            {
                if (isWhiteTurn)
                {
                    return "White";
                }
                else
                {
                    return "Black";
                }
            }
        }
        #endregion
        private Dictionary<string, Player> players;
        private Tile[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        //private int time;
        public Game()
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player());
            players.Add("black", new Player());
            board = new Tile[BOARDSIZE, BOARDSIZE];
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i, j] = new Tile(i, j, -1, this);
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

            board[0, 3].Value = 0;
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
            board[7, 3].Value = 0;



            isWhiteTurn = false;

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;
            ///

            showBoard();
            var list = getPlayableSquares(true);

            foreach (var player in list)
            {
                Console.WriteLine(player);  
            }

            Console.WriteLine(list.Count);



        }

        public void pause()
        {

        }

        public List<Tile> getPlayableSquares(bool isWhiteTurn)
        {
            List<Tile> potential = new List<Tile>();
            HashSet<Tile> playable = new HashSet<Tile>();
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
                            playable.Add(tile);
                            break;
                        }
                    }
                }
            }
            return playable.ToList<Tile>();
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

        public int getBlackScore()
        {
            throw new NotImplementedException();
        }

        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int getWhiteScore()
        {
            throw new NotImplementedException();
        }

        public bool isPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool playMove(int column, int line, bool isWhite)
        {
            if (isWhite)
            {
                board[line, column].Value = 0;
            }
            else
            {
                board[line, column].Value = 1;
            }
            return true;
        }
    }
}
