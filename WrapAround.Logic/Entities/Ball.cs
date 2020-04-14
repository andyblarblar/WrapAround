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
    public class Ball : ICollidable, IQuadrantHitbox
    {
        [IgnoreMember]
        public Vector2 Position;

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
        public Hitbox Hitbox { get; set; }

        [JsonIgnore]
        [IgnoreMember]
        public QuadrantController SegmentController { get; set; }

        [IgnoreMember]
        private readonly Vector2 _startingPosition;

        public Ball(Vector2 startingPosition, Vector2 rate)
        {
            SegmentController = new QuadrantController();
            Position = startingPosition;
            _startingPosition = startingPosition;
            _rate = rate;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + 10));
        }

        [SerializationConstructor]
        public Ball(Hitbox hitbox)
        {
            Hitbox = hitbox;
        }

        /// <summary>
        /// steps the ball physics forward.
        /// </summary>
        public void Update()
        {
            _physicsHistory = Position;

            Position.X += _rate.X * UpdateRate;
            Position.Y += _rate.Y * UpdateRate;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + 10));

        }

        public void Reset()
        {
            Position = _startingPosition;
            _rate = new Vector2(3, 0);
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
            Paddle p => HandlePaddleCollision,
            Block b when b.health > 0 => HandleBlockCollision,
            _ => (CollisionHandler)(_ => { })

        };

        private void HandlePaddleCollision(object paddle)
        {
            var realPaddle = paddle as Paddle;

            var relativeIntersectY = (realPaddle.Position.Y + (realPaddle.Height / 2)) - Position.Y;//assuming ball Y is accurate to the collision
            var normalizedRelativeIntersectionY = relativeIntersectY / (realPaddle.Height / 2);
            var bounceAngle = normalizedRelativeIntersectionY * MaxAngle;
            //look ma, i'm actually using math!
            _rate.X = realPaddle.IsOnRight ? Speed * -MathF.Cos(bounceAngle) : Speed * MathF.Cos(bounceAngle);
            _rate.Y = Speed * -MathF.Sin(bounceAngle);

            Update();//update to avoid getting stuck
        }

        private void HandleBlockCollision(object block)
        {
            var realBlock = block as Block;

            realBlock.Damage();

            if (Hitbox.IsOnTopOf(new Hitbox(_physicsHistory, new Vector2(_physicsHistory.X + 10, _physicsHistory.Y + 10)), realBlock.Hitbox))
            {
                _rate.Y *= -1;
            }

            if (Hitbox.IsToSideOf(new Hitbox(_physicsHistory, new Vector2(_physicsHistory.X+10,_physicsHistory.Y+10)), realBlock.Hitbox))
            {
                _rate.X *= -1;
            }

            Update();//update to avoid getting stuck
        }

        #endregion



    }



}

