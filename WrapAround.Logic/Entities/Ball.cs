using System;
using System.Numerics;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 10 Px by 10 Px ball (well, square) that bounces around
    /// </summary>
    public class Ball : ICollidable, IQuadrantHitbox
    {
        /// <summary>
        /// the position of the ball on the canvas
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// the position vector of the ball relative to the field "position". Velocity in pixels/ms
        /// </summary>
        private Vector2 rate;

        private const int SPEED = 1;

        private const float MAX_ANGLE = (float) (Math.PI * 5 / 12);// ~75 degrees

        private const float UPDATE_RATE = 16;//16ms update rate
        public Hitbox Hitbox { get; set; } 

        /// <inheritdoc />
        public QuadrantController SegmentController { get; set; } = new QuadrantController();

        private readonly Vector2 startingPosition;

        public Ball(Vector2 startingPosition, Vector2 rate)
        {
            position = startingPosition;
            this.startingPosition = startingPosition;
            this.rate = rate;
            Hitbox = new Hitbox(position, new Vector2(position.X + 10,position.Y + 10));
        }

        /// <summary>
        /// steps the ball physics forward.
        /// </summary>
        public void Update()
        {
            position.X += rate.X * UPDATE_RATE;
            position.Y += rate.Y * UPDATE_RATE;
            Hitbox = new Hitbox(position, new Vector2(position.X + 10, position.Y + 10));
            
        }

        public void Reset()
        {
            position = startingPosition;
            rate = new Vector2(-1,0);
        }


        #region CollisionLogic


        /// <summary>
        /// When colliding with an implementer of ICollidable.
        /// </summary>
        public async Task Collide(object collidedWith)
        {
            await Task.Run(() =>
            {
                var handler = FindCollisionHandler(collidedWith);
                handler.Invoke(collidedWith);
            });

        }

        private delegate void CollisionHandler(object collidedWith);

        /// <summary>
        /// Pattern matches collidedWith to find the proper method to call.
        /// </summary>
        /// <param name="collidedWith"></param>
        /// <returns></returns>
        private CollisionHandler FindCollisionHandler(object collidedWith) => collidedWith switch
        {
            Paddle p  => new CollisionHandler(HandlePaddleCollision),
            Block b when b.health != 0 => new CollisionHandler(HandleBlockCollision),
            _ => new CollisionHandler(_ => {})

        };

        private void HandlePaddleCollision(object paddle)
        {
            var realPaddle = paddle as Paddle;

            var relativeIntersectY = (realPaddle.Position.Y + (realPaddle.Height / 2)) - position.Y;//assuming ball Y is accurate to the collision
            var normalizedRelativeIntersectionY = relativeIntersectY / (realPaddle.Height / 2);
            var bounceAngle = normalizedRelativeIntersectionY * MAX_ANGLE;
            //look ma, i'm actually using math!
            rate.X = (float) (SPEED * Math.Cos(bounceAngle));
            rate.Y = (float) (SPEED * -Math.Sin(bounceAngle));

            Update();//update to avoid getting stuck
        }

        private void HandleBlockCollision(object block)
        {
            var realBlock = block as Block;

            realBlock.Damage();

            rate.Y *= -1;

            Update();//update to avoid getting stuck
        }

        #endregion


        
    }



}
    
