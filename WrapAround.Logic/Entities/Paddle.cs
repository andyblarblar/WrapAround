using System.Numerics;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A player controlled paddle of variable height but static 10 pixel width
    /// </summary>
    public class Paddle : IQuadrentHitbox, ICollidable
    {
        public int Id { get; }

        public bool IsOnRight { get; set; }

        public int GameId { get; }
        public Hitbox Hitbox { get; set; }

        public QuadrantController SegmentController { get; set; }

        public Vector2 Position { get; set; }

        private Vector2 StartingPosition { get; }

        public float Height { get; set; } 

        /// <summary>
        /// A unique id assigned to the player
        /// </summary>
        public string Hash { get; set; }

        private const float MAX_SIZE = 300;

        public Paddle(int gameId, int playerId, bool isOnRight, int playerTotalOnSide, string hash, Vector2 startingPosition)
        {
            Id = playerId;
            GameId = gameId;
            IsOnRight = isOnRight;
            Height = MAX_SIZE / playerTotalOnSide;
            Hash = hash;
            Position = startingPosition;
            StartingPosition = startingPosition;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + Height));
        }

        public void ResetLocation()
        {
           Update(StartingPosition);
        }

        public void AdjustSize(int numberOfPlayersOnSide)
        {
            Height = MAX_SIZE / numberOfPlayersOnSide;
        }

        /// <summary>
        /// Updates the position of both the players paddle and the underlying hitbox
        /// </summary>
        /// <param name="playerPosition"></param>
        public void Update(Vector2 playerPosition)
        {
            Position = playerPosition;
            Hitbox = new Hitbox(playerPosition, new Vector2(Position.X + 10, Position.Y + Height));

        }


        public Task Collide(object collided)
        {
            return Task.CompletedTask;//purposely empty, for future use
        }
    }
}