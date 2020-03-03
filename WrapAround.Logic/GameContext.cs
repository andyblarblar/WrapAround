using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic
{
    public class GameContext
    {
        public readonly int Id;

        public const int MaxPlayers = 16;

        private readonly List<GameMap> _maps;

        public List<Paddle> Players { get; }

        public Ball Ball { get; }

        public GameMap CurrentMap { get; set; }

        public ScoreBoard ScoreBoard { get; }

        public LobbyStates LobbyState { get; set; }

        public GameContext(int id, List<GameMap> maps)
        {
            Players = new List<Paddle>();
            CurrentMap = maps.Count == 1 ? maps[0] : maps[new Random().Next(0, maps.Count)];
            this.Id = id;
            this._maps = maps;
            Ball = new Ball(new Vector2(CurrentMap.CanvasSize.Item1 / 2, CurrentMap.CanvasSize.Item2 / 2),
                new Vector2(-1, 0));
            ScoreBoard = new ScoreBoard();
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

                var numOnSide = Players.Count(player => player.IsOnRight == isRightSide) + 1;

                var playerStartingPosition = isRightSide ? new Vector2(CurrentMap.CanvasSize.Item1 - 20, 0) : new Vector2(0, 0);

                var newPlayer = new Paddle(gameId: Id, playerId: Players.Count + 1, isRightSide, playerTotalOnSide: numOnSide, hash: hash, startingPosition: playerStartingPosition);//TODO create better Id derivation
                Players.Add(newPlayer);

                Players.ForEach((paddle => paddle.AdjustSize(numOnSide)));//adjust player sizes

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
                Players.Remove(Players.Single(paddle => paddle.Id == player.Id));

                var playersOnSide = Players.Count(paddle => paddle.IsOnRight == player.IsOnRight);
                Players.ForEach(paddle => paddle.AdjustSize(playersOnSide));//readjust sizes


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
                Ball.Update();
                await Ball.SegmentController.UpdateSegment(Ball.Hitbox);
                Players.AsParallel().ForAll(async paddle => await paddle.SegmentController.UpdateSegment(paddle.Hitbox));

                //Collision handle
                Players.AsParallel()
                    .Where(player => player.SegmentController.Segment.Contains(Ball.SegmentController.Segment.First()))//all paddles in same segment as ball
                    .Where(paddle => paddle.Hitbox.IsCollidingWith(Ball.Hitbox))//check for collision
                    .ForAll(async paddle => await CollideAsync(paddle, Ball));//Handle Collisions 

                CurrentMap?.Blocks.AsParallel()
                    .Where(block => block.SegmentController.Segment.Contains(Ball.SegmentController.Segment[0]))
                    .Where(block => block.Hitbox.IsCollidingWith(Ball.Hitbox))
                    .ForAll(async block => await CollideAsync(block, Ball));

                //Goal scoring
                if (CurrentMap.LeftGoal.Hitbox.IsCollidingWith(Ball.Hitbox))
                {
                    ScoreBoard.ScoreLeft();
                    Ball.Reset();
                }

                else if (CurrentMap.RightGoal.Hitbox.IsCollidingWith(Ball.Hitbox))
                {
                    ScoreBoard.ScoreRight();
                    Ball.Reset();
                }
                

                //WrapAround!
                Ball.Position.Y = Ball.Position.Y switch
                {
                    var pos when pos < 0 => CurrentMap.CanvasSize.Item2,
                    var pos when pos > CurrentMap.CanvasSize.Item2 => 1,
                    _ => Ball.Position.Y
                };

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
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static async Task CollideAsync(ICollidable obj1, ICollidable obj2)
        {
            await obj1.Collide(obj2);
            await obj2.Collide(obj1);

        }


        /// <summary>
        /// Resets state and finds a new map randomly.
        /// </summary>
        public void Reset()
        {
            Players.ForEach(paddle => paddle.ResetLocation());
            Ball.Reset();
            CurrentMap = _maps[new Random().Next(0, _maps.Count)];

            //update segment property
            Ball.SegmentController.CanvasSize = CurrentMap.CanvasSize;
            Players.ForEach(player => player.SegmentController.CanvasSize = CurrentMap.CanvasSize);

            ScoreBoard.Reset();
            LobbyState = IsLobbyFull() ? LobbyStates.InGame : LobbyStates.WaitingForPlayers;

        }


        public bool IsLobbyFull()
        {
            return Players.Count == MaxPlayers;
        }



    }
}
