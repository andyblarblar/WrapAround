using System;
using System.Numerics;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    public readonly struct GoalZone : IEquatable<GoalZone>
    {
        public Vector2 Position { get; }

        public Hitbox Hitbox { get; }

        public GoalZone(Vector2 position)
        {
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + 703));
        }

        public bool IsCollidingWith(in Hitbox hitbox) => Hitbox.IsCollidingWith(in hitbox);

        public bool Equals(GoalZone other)
        {
            return Hitbox.Equals(other.Hitbox);
        }

        public override bool Equals(object obj)
        {
            return obj is GoalZone other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Hitbox.GetHashCode();
        }

        public static bool operator ==(GoalZone left, GoalZone right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GoalZone left, GoalZone right)
        {
            return !left.Equals(right);
        }
    }
}