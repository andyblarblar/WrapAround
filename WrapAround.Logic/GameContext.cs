using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WrapAround.Logic.Entities;

namespace WrapAround.Logic
{
    public class GameContext
    {
        public readonly int id;

        public const int MAX_PLAYERS = 16;

        private List<GameMap> maps;

        private List<Paddle> players;

        private Ball ball;

        private GameMap currentMap { get; set; }


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

            throw new NotImplementedException();

        }

        public bool IsLobbyFull()
        {
            return players.Count !< 16;
        }



    }
}
