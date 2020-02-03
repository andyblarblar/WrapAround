﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;

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




        /// <summary>
        /// adds a player to the game.
        /// </summary>
        /// <returns>the id of the player, -1 if lobby is full.</returns>
        public async Task<int> AddPlayer()
        {
            return await Task.Run(() =>
            {
                if (IsLobbyFull()) return -1;

                var newPlayer = new Paddle(gameId: id, playerId: players.Count);
                players.Add(newPlayer); //May need to change playerId derivation

                return newPlayer.id;
            });
        }

        /// <summary>
        /// Steps the physics forward, checking for conditions.
        /// </summary>
        public void Update()
        {
            throw new NotImplementedException();
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
            return players.Count !< 16;
        }



    }
}
