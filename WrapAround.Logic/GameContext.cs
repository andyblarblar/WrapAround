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
        
        private List<GameMap> maps;

        public List<Paddle> players { get; }

        private Ball ball;

        private GameMap currentMap { get; set; }

        private ScoreBoard scoreBoard;

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
        public async Task<int> AddPlayer(bool isRightSide)
        { 
            return await Task.Run(() =>
            {
                if (IsLobbyFull()) return -1;

                var numOnSide = players.Count(player => player.isOnRight == isRightSide) + 1;

                var newPlayer = new Paddle(gameId: id, playerId: players.Count, isRightSide, playerTotalOnSide: numOnSide);
                players.Add(newPlayer); 

                players.ForEach((paddle => paddle.AdjustSize(numOnSide)));//adjust player sizes

                return newPlayer.id;
            });
        }

        /// <summary>
        /// Steps the physics forward, checking for conditions.
        /// </summary>
        public async Task Update()
        {
            await Task.Run(() =>
            {
                if (IsLobbyFull()) LobbyState = LobbyStates.InGame;
                if (LobbyState == LobbyStates.WaitingForPlayers) return; //do nothing if the lobby is still waiting

                //TODO impliment hitboxes to detect collisions, then handle. (blocks, goalzone ect). Players are already done.

                ball.Update();








            });

        }

        /// <summary>
        /// Resets state and finds a new map randomly.
        /// </summary>
        public void Reset()
        {
            players.ForEach((paddle => paddle.ResetLocation()));
            ball.Reset();
            currentMap = maps[new Random().Next(0, maps.Count)];
            scoreBoard.Reset();
        }

        public bool IsLobbyFull()
        {
            return players.Count !< MAX_PLAYERS;
        }



    }
}
