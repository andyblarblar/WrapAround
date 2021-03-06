﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;
using MessagePack;

namespace WrapAround.Logic
{
    /// <summary>
    /// Encapsulates the state of a wraparound lobby
    /// </summary>
    [MessagePackObject]
    public class GameContext
    {
        [IgnoreMember]
        public readonly int Id;

        [IgnoreMember]
        public const int MaxPlayers = 16;

        [IgnoreMember]
        private readonly List<GameMap> _maps;

        [Key("players")]
        public List<Paddle> Players { get; }

        [Key("ball")] 
        public Ball Ball;

        [Key("currentMap")]
        public GameMap CurrentMap { get; set; }

        /// <summary>
        /// Bit marked when blocks have changed during an update. 
        /// </summary>
        [Key("blocksHaveChanged")]
        public bool BlocksHaveChanged { get; set; } = true;

        private Block[] OldBlocks { get; set; } 

        [Key("scoreBoard")]
        public ScoreBoard ScoreBoard { get; }

        [Key("lobbyState")]
        public LobbyStates LobbyState { get; set; }

        [IgnoreMember]
        private Timer UpdateTimer { get; }

        public GameContext(int id, List<GameMap> maps)
        {
            Players = new List<Paddle>();
            CurrentMap = maps.Count == 1 ? maps[0] : maps[new Random().Next(0, maps.Count)];
            this.Id = id;
            this._maps = maps;
            Ball = new Ball(new Vector2(CurrentMap.CanvasSize.Item1 / 2, CurrentMap.CanvasSize.Item2 / 2),
                new Vector2(3, 0));
            ScoreBoard = new ScoreBoard();
            LobbyState = LobbyStates.WaitingForPlayers;
            OldBlocks = new Block[CurrentMap.Blocks.Length - 1];

            UpdateTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = 5000
            };

            //start the game if you have at least 2 players after 20 seconds
            UpdateTimer.Elapsed += (sender, args) => LobbyState = Players.Count > 1 ? LobbyStates.InGame : LobbyStates.WaitingForPlayers;
        }

        /// <summary>
        /// for messagepack serialization
        /// </summary>
        [SerializationConstructor]
        public GameContext(List<Paddle> players, Ball ball, GameMap currentMap, ScoreBoard scoreBoard, LobbyStates lobbyState)
        {
            Players = players;
            Ball = ball;
            CurrentMap = currentMap;
            ScoreBoard = scoreBoard;
            LobbyState = lobbyState;
            BlocksHaveChanged = false;
        }

        /// <summary>
        /// adds a player to the game, resizing paddles on team as needed.
        /// </summary>
        /// <returns>the id of the player, -1 if lobby is full.</returns>
        public async Task<int> AddPlayer(bool isRightSide, string hash)
        {
            return await Task.Run(() =>
            {
                if (IsLobbyFull()) return -1;

                var ran = new Random();

                var numOnSide = Players.Count(player => player.IsOnRight == isRightSide) + 1;

                var playerStartingPosition =
                    isRightSide ? new Vector2(CurrentMap.CanvasSize.Item1 - 20, 0) : new Vector2(20, 0);

                try
                {
                    var newPlayer = new Paddle(
                        gameId: Id,
                        #pragma warning disable CA1305 // Specify IFormatProvider
                        playerId: int.Parse(hash, NumberStyles.Any) ^ ran.Next(),//we should prob fix this lol 
                        #pragma warning restore CA1305 // Specify IFormatProvider
                        isOnRight: isRightSide,
                        playerTotalOnSide: numOnSide, 
                        hash: hash,
                        startingPosition: playerStartingPosition); 

                    Players.Add(newPlayer);

                    //adjust player sizes on the same side
                    Players.ForEach(paddle =>
                    {
                        if (paddle.IsOnRight == isRightSide)
                        {
                            paddle.AdjustSize(numOnSide);
                        }
                    });

                    return newPlayer.Id;
                }
                catch (Exception)//if something goes wrong in parsing
                {
                    return -1;
                }

            });
        }

        /// <summary>
        /// Removes the specified player from the lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemovePlayer(Paddle player)
        {
            await Task.Run(() =>
            {
                Players.Remove(Players.Single(paddle => paddle.Id == player.Id));

                var playersOnSide = Players.Count(paddle => paddle.IsOnRight == player.IsOnRight);
                Players.ForEach(paddle =>
                {
                    if (paddle.IsOnRight == player.IsOnRight)
                    {
                        paddle.AdjustSize(playersOnSide);
                    }
                }); //ajust sizes of paddles on the same size

            });
        }

        /// <summary>
        /// Steps the physics forward, checking for conditions.
        /// </summary>
        public async Task Update()
        {
            await Task.Run(() =>
            {
                if (LobbyState == LobbyStates.WonByLeft || LobbyState == LobbyStates.WonByRight)//check for win
                {
                    Reset();
                    return;
                }

                if (IsLobbyFull()) LobbyState = LobbyStates.InGame;
                if (LobbyState == LobbyStates.WaitingForPlayers) return; //do nothing if the lobby is still waiting

               
                    Ball.Update();

                    //Collision handle
                    foreach (var player in Players) 
                    {
                        if (player.IsCollidingWith(in Ball.Hitbox))
                        {
                            CollidePaddleWithBall(player, ref Ball);
                        }
                    }

                    if (CurrentMap.Blocks.Length > 0) 
                    {
                        //needs to be for loop for ref block
                        for (var i = 0; i < (CurrentMap?.Blocks).Length; i++)
                        {
                            ref var block = ref (CurrentMap?.Blocks)[i];
                            if (block.IsCollidingWith(in Ball.Hitbox))
                            {
                                CollideBlockWithBall(ref block, ref Ball);
                            }
                        }

                        //If blocks have changed sense last iteration, mark diff bit
                        BlocksHaveChanged = CurrentMap.Blocks.Except(OldBlocks).Any();
                        
                        //update blocks for diffing
                        OldBlocks = CurrentMap.Blocks.Copy();

                    }

                    //Goal scoring 
                    var actionIfScored = CurrentMap.CheckForGoal(in Ball.Hitbox) switch
                    {
                        var (leftScored, _) when leftScored => () =>
                        {
                            ScoreBoard.ScoreLeft();
                            Ball.Reset();
                        },

                        var (_, rightScored) when rightScored => () =>
                        {
                            ScoreBoard.ScoreRight();
                            Ball.Reset();
                        },

                        _ => (Action) (() => {})
                    };

                    actionIfScored.Invoke();

                    //WrapAround!
                    Ball.KeepInBounds(CurrentMap.CanvasSize);

                //check for wins
                var actionIfWon = ScoreBoard.IsWon() switch
                {
                    var (leftWon, _) when leftWon => () => { LobbyState = LobbyStates.WonByLeft; }
                    ,
                    var (_, rightWon) when rightWon => () => { LobbyState = LobbyStates.WonByRight; }
                    ,
                    _ => (Action)(() => { })

                };
                actionIfWon.Invoke();

            });

        }

        /// <summary>
        /// Simply calls the collision handler on each object
        /// </summary>
        public static void CollidePaddleWithBall(Paddle paddle, ref Ball ball)
        { 
             ball.Collide(paddle);
        } 
        
        /// <summary>
        /// Simply calls the collision handler on each object
        /// </summary>
        public static void CollideBlockWithBall(ref Block block, ref Ball ball)
        { 
             ball.Collide(in block);
             block = Block.DamageBlock(in block);
        }


        /// <summary>
        /// Resets state and finds a new map randomly.
        /// </summary>
        public void Reset()
        {
            Players.ForEach(paddle => paddle.ResetLocation());
            Ball.Reset();

            //reset for the future
            CurrentMap.Reset();
            CurrentMap = _maps[new Random().Next(0, _maps.Count)];

            ScoreBoard.Reset();
            LobbyState = IsLobbyFull() ? LobbyStates.InGame : LobbyStates.WaitingForPlayers;

        }


        public bool IsLobbyFull()
        {
            return Players.Count == MaxPlayers;
        }



    }
}
