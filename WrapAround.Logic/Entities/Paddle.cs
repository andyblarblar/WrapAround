using System;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;

namespace WrapAround.Logic.Entities
{
    public class Paddle
    {
        public int id { get; }

        public bool isOnRight { get; set; }

        public int gameId { get; }

        public Vector2 position { get; set; }

        public float height { get; set; }

        private const float MAX_SIZE = 100;//TODO finialise size
        public Paddle(int gameId, int playerId, bool isOnRight, int playerTotalOnSide)
        {
            id = playerId;
            this.gameId = gameId;
            this.isOnRight = isOnRight;
            height = MAX_SIZE / playerTotalOnSide;
        }


        public void ResetLocation()
        {
            throw new NotImplementedException();
        }

        public void AdjustSize(int numberOfPlayersOnSide)
        {
            height = MAX_SIZE / numberOfPlayersOnSide;
        }





    }
}