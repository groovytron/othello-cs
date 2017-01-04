﻿using System;
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

        private Dictionary<string, Player> players;
        private int[,] board;
        private const int BOARDSIZE = 8;
        private bool isWhiteTurn;
        //private int time;


        public Game()
        {
            players = new Dictionary<string, Player>();
            players.Add("white",new Player());
            players.Add("black", new Player());
            board = new int[BOARDSIZE, BOARDSIZE];
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    board[i,j] = -1;
                }
            }
          
        }

        private void showBoard()
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    Console.Write($"\t{board[i,j]}");
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
                    board[i, j] = b[h++];
                }
            }

        }

        public void newGame()
        {
            foreach (var player in players)
            {
                player.Value.reset();
            }

            playMove(3, 3, true);
            playMove(4, 4, true);
            playMove(4, 3, false);
            playMove(3, 4, false);

            isWhiteTurn = false;

            foreach (var player in players)
            {
                player.Value.Score = 2;
                //player.Value.Time = 1800000;
            }

            ///time = DateTime.Now.Millisecond;

            showBoard();
            showPlayerStatus();

        }

        public void pause()
        {

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
                board[line, column] = 0;
            }
            else
            {
                board[line, column] = 1;
            }
            return true;
        }
    }
}
