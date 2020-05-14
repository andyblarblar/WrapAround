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
    /// A player controlled paddle of variable height but static 10 pixel width
    /// </summary>
    [MessagePackObject]
    public class Paddle : ICollidable, IEquatable<Paddle> //Note: needs to be class for use in async methods upstream
    {
        [Key("id")]
        public int Id { get; set; }

        [Key("isOnRight")]
        public bool IsOnRight { get; set; }

        [Key("gameId")]
        public int GameId { get; set; }

        [Key("hitbox")]
        public Hitbox Hitbox { get; set; }

        [JsonIgnore]
        [IgnoreMember]
        public Vector2 Position { get; set; }

        [IgnoreMember]
        private Vector2 StartingPosition { get; }

        [Key("height")]
        public float Height { get; set; }

        /// <summary>
        /// A unique id assigned to the player
        /// </summary>
        [JsonIgnore]
        [Key("hash")]
        public string Hash { get; set; }

        [IgnoreMember]
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

        /// <summary>
        /// for MessagePack
        /// </summary>
        [SerializationConstructor]
        public Paddle(int gameId, int id, bool isOnRight, string hash, float height, Hitbox hitbox)
        {
            Id = id;
            GameId = gameId;
            IsOnRight = isOnRight;
            Hash = hash;
            Hitbox = hitbox;
            Height = height;
            Position = hitbox.TopLeft;
            StartingPosition = Position;
        }

        public Paddle()
        {

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


        public override string ToString()
        {
            return $"{Hitbox}";
        }

        public bool IsCollidingWith(in Hitbox hitbox) => Hitbox.IsCollidingWith(in hitbox);

        public void Collide<T>(in T collidedWith)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Paddle other)
        {
            return Id == other.Id && IsOnRight == other.IsOnRight && GameId == other.GameId && Hitbox.Equals(other.Hitbox) && Height.Equals(other.Height) && Hash == other.Hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Paddle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, IsOnRight, GameId, Hitbox, Height, Hash);
        }

        public static bool operator ==(Paddle left, Paddle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Paddle left, Paddle right)
        {
            return !left.Equals(right);
        }
    }
}