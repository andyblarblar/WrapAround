using System;
using System.Numerics;
using System.Text.Json.Serialization;
using MessagePack;

namespace WrapAround.Logic.Util
{
    /// <summary>
    /// A rectangle representing a hitbox
    /// </summary>
    [MessagePackObject]
    public readonly struct Hitbox : IEquatable<Hitbox>
    {
        [JsonConverter(typeof(Vector2Converter))]
        [Key("topLeft")]
        public Vector2 TopLeft { get; }

        [JsonConverter(typeof(Vector2Converter))]
        [Key("bottomRight")]
        public Vector2 BottomRight { get; }

        [JsonConstructor]
        public Hitbox(Vector2 topLeft, Vector2 bottomRight)
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        /// <summary>
        /// Checks if two rectangles are intersecting by checking if either Rectangle is above
        /// or to the left of the other rectangle.
        /// </summary>
        /// <param name="hitbox">other hitbox</param>
        public bool IsCollidingWith(in Hitbox hitbox)
        {
            //if one rectangle is to the right of the other (one rectangles point is to the right of both points of the other rectangle)
            if (TopLeft.X > hitbox.BottomRight.X && TopLeft.X > hitbox.TopLeft.X || hitbox.TopLeft.X > BottomRight.X && hitbox.TopLeft.X > TopLeft.X)
            {
                return false;
            }

            //if one rectangle is above the other (one rectangles top left point is above both points of the other rectangle)
            if (TopLeft.Y < hitbox.TopLeft.Y && BottomRight.Y < hitbox.TopLeft.Y || hitbox.TopLeft.Y < TopLeft.Y && hitbox.BottomRight.Y < TopLeft.Y)
            {
                return false;
            }

            //now the rectangles must be intersecting
            return true;

        }

        /// <summary>
        /// Checks if the hitbox is a valid rectangle, IE: the top left point is the top left and the
        /// bottom right point is bottom right. Collision does not work if invalid rectangles are used.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return TopLeft.X > BottomRight.X && TopLeft.Y > BottomRight.Y;
        }

        /// <summary>
        /// returns true if either rectangle is to the side of the other
        /// </summary>
        /// <param name="hitbox1"></param>
        /// <param name="hitbox2"></param>
        /// <returns></returns>
        public static bool IsToSideOf(in Hitbox hitbox1, in Hitbox hitbox2)
        {
            //if one rectangle is to the right of the other (one rectangles point is to the right of both points of the other rectangle)
            return hitbox1.TopLeft.X > hitbox2.BottomRight.X && hitbox1.TopLeft.X > hitbox2.TopLeft.X ||
                   hitbox2.TopLeft.X > hitbox1.BottomRight.X && hitbox2.TopLeft.X > hitbox1.TopLeft.X;
        }

        /// <summary>
        /// returns true if either rectangle is on top of the other
        /// </summary>
        /// <param name="hitbox1"></param>
        /// <param name="hitbox2"></param>
        /// <returns></returns>
        public static bool IsOnTopOf(in Hitbox hitbox1, in Hitbox hitbox2)
        {
            //if one rectangle is above the other (one rectangles top left point is above both points of the other rectangle)
            return hitbox1.TopLeft.Y < hitbox2.TopLeft.Y && hitbox1.BottomRight.Y < hitbox2.TopLeft.Y ||
                   hitbox2.TopLeft.Y < hitbox1.TopLeft.Y && hitbox2.BottomRight.Y < hitbox1.TopLeft.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Hitbox other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TopLeft, BottomRight);
        }

        public static bool operator == (Hitbox left, Hitbox right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Hitbox left, Hitbox right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"TL:{TopLeft}, BR:{BottomRight}";
        }

        public bool Equals(Hitbox other)
        {
            return TopLeft.Equals(other.TopLeft) && BottomRight.Equals(other.BottomRight);
        }
    }
}