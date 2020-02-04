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

        public Paddle(int gameId, int playerId, bool isOnRight)
        {
            id = playerId;
            this.gameId = gameId;
            this.isOnRight = isOnRight;
            //TODO create position and size
        }





        public void ResetLocation()
        {
            throw new NotImplementedException();
        }






    }
}