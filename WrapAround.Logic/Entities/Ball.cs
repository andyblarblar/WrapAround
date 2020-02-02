using System;
using System.Globalization;
using System.Numerics;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    public class Ball : ICollidable
    {
        private Vector2 position;

        private Vector2 rate;

        public Ball(Vector2 startingPosition, Vector2 rate)
        {
            this.position = startingPosition;
            this.rate = rate;

        }

        /// <summary>
        /// steps the ball physics forward.
        /// </summary>
        public void Update()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When colliding with an implementer of ICollidable.
        /// </summary>
        public void Collide(object collidedWith)
        {
            var handler = FindCollisionHandler(collidedWith);
            handler.Invoke(collidedWith);

        }

        private delegate void CollisionHandler(object collidedWith);

        /// <summary>
        /// Pattern matches collidedWith to find the proper method to call.
        /// </summary>
        /// <param name="collidedWith"></param>
        /// <returns></returns>
        private CollisionHandler FindCollisionHandler(object collidedWith) => collidedWith switch
        {
            Paddle p => new CollisionHandler(HandlePaddleCollision)
            //TODO impliment all collision handlers.

        };

        private void HandlePaddleCollision(object paddle)
        {

            throw new NotImplementedException();

        }



    }



}
    
