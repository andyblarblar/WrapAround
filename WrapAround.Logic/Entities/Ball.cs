using System;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;
using MessagePack;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 10 Px by 10 Px ball (well, square) that bounces around
    /// </summary>
    [MessagePackObject]
    public struct Ball : ICollidable, IEquatable<Ball>
    {
        [IgnoreMember]
        private Vector2 _position;

        [IgnoreMember]
        private Vector2 _rate;

        [IgnoreMember]
        private const int Speed = 5;

        [IgnoreMember]
        private const float MaxAngle = MathF.PI * 5 / 12;// ~75 degrees

        [IgnoreMember]
        private const float UpdateRate = 1.6f;//16ms update rate

        [IgnoreMember]
        private Vector2 _physicsHistory;

        [Key("hitbox")] 
        public Hitbox Hitbox;

        [IgnoreMember]
        private readonly Vector2 _startingPosition;

        public Ball(Vector2 startingPosition, Vector2 rate)
        {
            _position = startingPosition;
            _startingPosition = startingPosition;
            _rate = rate;
            Hitbox = new Hitbox(_position, new Vector2(_position.X + 10, _position.Y + 10));
            _physicsHistory = startingPosition;
        }

        /// <summary>
        /// Constructor for serialization, DO NOT USE!  
        /// </summary>
        [SerializationConstructor]
        public Ball(Hitbox hitbox)
        {
            Hitbox = hitbox;
            _position = hitbox.TopLeft;
            _startingPosition = _position;
            _rate = _position;
            _physicsHistory = _position;
        }

        /// <summary>
        /// steps the ball physics forward.
        /// </summary>
        public void Update()
        {
            _physicsHistory = _position;

            _position.X += _rate.X * UpdateRate;
            _position.Y += _rate.Y * UpdateRate;
            Hitbox = new Hitbox(_position, new Vector2(_position.X + 10, _position.Y + 10));

        }

        public void Reset()
        {
            _position = _startingPosition;
            _rate = new Vector2(3, 0);
        }

        /// <summary>
        /// Checks if the ball is above or below the playable canvas, wraps around if it is.
        /// </summary>
        public void KeepInBounds(in (int,int) canvasSize)
        {
            var (_, y) = canvasSize;

            _position.Y = _position.Y switch
            {
                var pos when pos < 0 => y,
                var pos when pos > y => 1,
                _ => _position.Y
            };

        }


        #region CollisionLogic


        /// <summary>
        /// When colliding with an implementer of ICollidable.
        /// </summary>
        public void Collide<T>(in T collidedWith)
        {
            FindCollisionHandler(in collidedWith);
        }

        /// <summary>
        /// Pattern matches collidedWith to find the proper method to call.
        /// </summary>
        /// <param name="collidedWith"></param>
        /// <returns></returns>
        private void FindCollisionHandler<T>(in T collidedWith)
        {
            switch (collidedWith)
            {
                case Paddle p:
                    HandlePaddleCollision(in p);
                    break;

                case Block b when b.Health > 0:
                    HandleBlockCollision(in b);
                    break;

                default:
                    break;//this means we collided with a block that has 0 health
            }
        }

        private void HandlePaddleCollision(in Paddle paddle)
        {
            var relativeIntersectY = (paddle.Position.Y + (paddle.Height / 2)) - _position.Y;//assuming ball Y is accurate to the collision
            var normalizedRelativeIntersectionY = relativeIntersectY / (paddle.Height / 2);
            var bounceAngle = normalizedRelativeIntersectionY * MaxAngle;
            //look ma, i'm actually using math!
            _rate.X = paddle.IsOnRight ? Speed * -MathF.Cos(bounceAngle) : Speed * MathF.Cos(bounceAngle);
            _rate.Y = Speed * -MathF.Sin(bounceAngle);

            Update();//update to avoid getting stuck
        }

        private void HandleBlockCollision(in Block block)
        {

            if (Hitbox.IsOnTopOf(new Hitbox(_physicsHistory, new Vector2(_physicsHistory.X + 10, _physicsHistory.Y + 10)), block.Hitbox))
            {
                _rate.Y *= -1;
            }

            if (Hitbox.IsToSideOf(new Hitbox(_physicsHistory, new Vector2(_physicsHistory.X+10,_physicsHistory.Y+10)), block.Hitbox))
            {
                _rate.X *= -1;
            }

            Update();//update to avoid getting stuck
        }

        public override bool Equals(object obj)
        {
            return obj is Ball other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Hitbox.GetHashCode();
        }

        public static bool operator ==(Ball left, Ball right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ball left, Ball right)
        {
            return !(left == right);
        }

        #endregion

        public bool Equals(Ball other)
        {
            return Hitbox.Equals(other.Hitbox);
        }
    }



}

