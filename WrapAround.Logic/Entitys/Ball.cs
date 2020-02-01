using System.Globalization;
using System.Numerics;

namespace WrapAround.Logic.Entitys
{
    public class Ball : ICollidable
    {
        private Vector2 position { get; set; }

        private Vector2 rate { get; set; }



        /// <summary>
        /// When colliding with an implimenter of ICollidable.
        /// </summary>
        public void Collide(object collidedWith)
        {

        }
    }
    
}