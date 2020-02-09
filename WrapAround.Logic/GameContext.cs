using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;
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
            ball = new Ball(new Vector2(currentMap.canvasSize.Item1,currentMap.canvasSize.Item2), new Vector2(-1,0));
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

                var numOnSide = players.Count(player => player.isOnRight == isRightSide) + 1;

                var newPlayer = new Paddle(gameId: id, playerId: players.Count, isRightSide, playerTotalOnSide: numOnSide, hash: hash);
                players.Add(newPlayer); 

                players.ForEach((paddle => paddle.AdjustSize(numOnSide)));//adjust player sizes

                return newPlayer.id;
            });
        }

        /// <summary>
        /// Removes the specified player from the lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemovePlayer(Paddle player)
        {
            await Task.Run((() =>
            {
                players.Remove(players.AsParallel().Single(paddle => paddle.id == player.id && paddle.hash == player.hash));

            }));
        }

        /// <summary>
        /// Steps the physics forward, checking for conditions.
        /// </summary>
        public async Task Update()
        {
            await Task.Run(() =>
            {
                if (LobbyState == LobbyStates.WonByLeft || LobbyState == LobbyStates.WonByRight)
                {
                    Reset();
                    return;
                }

                if (IsLobbyFull()) LobbyState = LobbyStates.InGame;
                if (LobbyState == LobbyStates.WaitingForPlayers) return; //do nothing if the lobby is still waiting

                //TODO impliment hitboxes to detect collisions, then handle. (blocks, goalzone ect). Players are already done.

                ball.Update();
                //Do rest of updates

                //then collision detect



                var actionIfWon = scoreBoard.isWon() switch
                {
                    var (leftWon, _) when leftWon => () => { LobbyState = LobbyStates.WonByLeft; },
                    var (_, rightWon) when rightWon => () => { LobbyState = LobbyStates.WonByRight; },
                    _ => (Action) (() => {})

                };
                actionIfWon.Invoke();



            });

        }

        /// <summary>
        /// Resets state and finds a new map randomly.
        /// </summary>
        public void Reset()
        {
            players.ForEach(paddle => paddle.ResetLocation());
            ball.Reset();
            currentMap = maps[new Random().Next(0, maps.Count)];
            scoreBoard.Reset();
            LobbyState = IsLobbyFull() ? LobbyStates.InGame : LobbyStates.WaitingForPlayers;

        }

        public bool IsLobbyFull()
        {
            return players.Count !< MAX_PLAYERS;
        }



    }
}
