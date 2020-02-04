using System;
using System.Globalization;
using System.Numerics;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    public class Ball : ICollidable
    {
        /// <summary>
        /// the position of the ball on the canvas
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// the position vector of the ball relative to the field "position". Velocity in pixels/ms
        /// </summary>
        private Vector2 rate;

        private const int SPEED = 5;//TODO finalize when units are defined

        private const float MAX_ANGLE = (float) (Math.PI * 5 / 12);// ~75 degrees

        private const float UPDATE_RATE = 16;//16ms update rate
        
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
            position.X = rate.X * UPDATE_RATE;
            position.Y = rate.Y * UPDATE_RATE;

        }

        public void Reset()
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
            var realPaddle = paddle as Paddle;

            var relativeIntersectY = (realPaddle.position.Y + (realPaddle.height / 2)) - position.Y;//assuming ball Y is accurate to the collision
            var normalizedRelativeIntersectionY = relativeIntersectY / (realPaddle.height / 2);
            var bounceAngle = normalizedRelativeIntersectionY * MAX_ANGLE;
            //look ma, i'm actually using math!
            rate.X = (float) (SPEED * Math.Cos(bounceAngle));
            rate.Y = (float) (SPEED * -Math.Sin(bounceAngle));

        }



    }



}
    
