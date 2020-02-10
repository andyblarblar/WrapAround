using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    public class Paddle
    {
        public int Id { get; }

        public bool IsOnRight { get; set; }

        public int GameId { get; }

        public Vector2 Position { get; set; }

        public float Height { get; set; } 

        /// <summary>
        /// A unique id assigned to the player
        /// </summary>
        public string Hash { get; set; }

        private const float MAX_SIZE = 300;

        public Paddle(int gameId, int playerId, bool isOnRight, int playerTotalOnSide, string hash, Vector2 startingPosition)
        {
            Id = playerId;
            this.GameId = gameId;
            this.IsOnRight = isOnRight;
            Height = MAX_SIZE / playerTotalOnSide;
            this.Hash = hash;
            Position = startingPosition;
        }

        public void ResetLocation()
        {
            throw new NotImplementedException();
        }

        public void AdjustSize(int numberOfPlayersOnSide)
        {
            Height = MAX_SIZE / numberOfPlayersOnSide;
        }





    }
}