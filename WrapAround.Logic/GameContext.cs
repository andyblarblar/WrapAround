using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic
{
    public class GameContext
    {
        public readonly int id;

        public const int MAX_PLAYERS = 16;
        
        private readonly List<GameMap> maps;

        public List<Paddle> players { get; }

        private readonly Ball ball;

        private GameMap currentMap { get; set; }

        private readonly ScoreBoard scoreBoard;

        public LobbyStates LobbyState;

        public GameContext(int id, List<GameMap> maps)
        {
            currentMap = maps[new Random().Next(0, maps.Count)];
            this.id = id;
            this.maps = maps;
            ball = new Ball(new Vector2(currentMap.CanvasSize.Item1 / 2,currentMap.CanvasSize.Item2 / 2), new Vector2(-1,0));
            scoreBoard = new ScoreBoard();
            LobbyState = LobbyStates.WaitingForPlayers;
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

                var numOnSide = players.Count(player => player.IsOnRight == isRightSide) + 1;

                var playerStartingPosition = isRightSide ? new Vector2(currentMap.CanvasSize.Item1 - 20, 0) : new Vector2(0, 0);

                var newPlayer = new Paddle(gameId: id, playerId: players.Count, isRightSide, playerTotalOnSide: numOnSide, hash: hash, startingPosition: playerStartingPosition);
                players.Add(newPlayer); 

                players.ForEach((paddle => paddle.AdjustSize(numOnSide)));//adjust player sizes

                return newPlayer.Id;
            }).ConfigureAwait(true);
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
                players.Remove(players.AsParallel().Single(paddle => paddle.Id == player.Id && paddle.Hash == player.Hash));

                var playersOnSide = players.Count(paddle => paddle.IsOnRight == player.IsOnRight);
                players.ForEach(paddle => paddle.AdjustSize(playersOnSide));//readjust sizes


            }).ConfigureAwait(true);
        }

        /// <summary>
        /// Steps the physics forward, checking for conditions.
        /// </summary>
        public async Task Update()
        {
            await Task.Run(async () =>
            {
                if (LobbyState == LobbyStates.WonByLeft || LobbyState == LobbyStates.WonByRight)//check for win
                {
                    Reset();
                    return;
                }

                if (IsLobbyFull()) LobbyState = LobbyStates.InGame;
                if (LobbyState == LobbyStates.WaitingForPlayers) return; //do nothing if the lobby is still waiting

                //update segments
                ball.Update();
                await ball.SegmentController.UpdateSegment(ball.Hitbox).ConfigureAwait(true);
                players.AsParallel().ForAll(async paddle => await paddle.SegmentController.UpdateSegment(paddle.Hitbox));

                //Collision handle
                players.AsParallel()
                    .Where(player => player.SegmentController.Segment.Contains(ball.SegmentController.Segment[0]))//all paddles in same segment as ball
                    .Where(paddle => paddle.Hitbox.IsCollidingWith(ball.Hitbox))//check for collision
                    .ForAll(async paddle => await CollideAsync(paddle, ball));//Handle Collisions 

                currentMap.Blocks.AsParallel()
                    .Where(block => block.SegmentController.Segment.Contains(ball.SegmentController.Segment[0]))
                    .Where(block => block.Hitbox.IsCollidingWith(ball.Hitbox))
                    .ForAll(async block => await CollideAsync(block,ball));

                //Goal scoring
                if (currentMap.LeftGoal.SegmentController.Segment.Contains(ball.SegmentController.Segment[0]))
                {
                    if (currentMap.LeftGoal.Hitbox.IsCollidingWith(ball.Hitbox))
                    {
                        scoreBoard.ScoreLeft();
                        ball.Reset();
                    }
                }
                else if (currentMap.RightGoal.SegmentController.Segment.Contains(ball.SegmentController.Segment[0]))
                {
                    if (currentMap.RightGoal.Hitbox.IsCollidingWith(ball.Hitbox))
                    {
                        scoreBoard.ScoreRight();
                        ball.Reset();
                    }
                }

                //TODO check if ball should wraparound TM

                var actionIfWon = scoreBoard.IsWon() switch
                {
                    var (leftWon, _) when leftWon => () => { LobbyState = LobbyStates.WonByLeft; },
                    var (_, rightWon) when rightWon => () => { LobbyState = LobbyStates.WonByRight; },
                    _ => (Action) (() => {})

                };
                actionIfWon.Invoke();

            });

        }

        /// <summary>
        /// Simply calls the collision handler on each object
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public async Task CollideAsync(ICollidable obj1, ICollidable obj2)
        { 
            await obj1.Collide(obj2);
            await obj2.Collide(obj1);

        }



        /// <summary>
        /// Resets state and finds a new map randomly.
        /// </summary>
        public void Reset()
        {
            players.ForEach(paddle => paddle.ResetLocation());
            ball.Reset();
            currentMap = maps[new Random().Next(0, maps.Count)];

            //update segment property
            ball.SegmentController.CanvasSize = currentMap.CanvasSize;
            players.ForEach(player => player.SegmentController.CanvasSize = currentMap.CanvasSize);

            scoreBoard.Reset();
            LobbyState = IsLobbyFull() ? LobbyStates.InGame : LobbyStates.WaitingForPlayers;

        }

        public bool IsLobbyFull()
        {
            return players.Count !< MAX_PLAYERS;
        }



    }
}
