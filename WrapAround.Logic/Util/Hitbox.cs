using System;
using System.Numerics;
using System.Text.Json.Serialization;
using WrapAround;

namespace WrapAround.Logic.Util
{
    /// <summary>
    /// A rectangle representing a hitbox
    /// </summary>
    public struct Hitbox
    {
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 TopLeft { get; }

        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 BottomRight { get; }

        public Hitbox(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        /// <summary>
        /// Checks if two rectangles are intersecting by checking if either Rectangle is above
        /// or to the left of the other rectangle.
        /// </summary>
        /// <param name="hitbox">other hitbox</param>
        public bool IsCollidingWith(in Hitbox hitbox)
        {
            //if one rectangle is to the left of the other
            if (TopLeft.X > hitbox.BottomRight.X || hitbox.TopLeft.X > BottomRight.X)
            {
                return false;
            }

            //if one rectangle is above the other
            if (TopLeft.Y < hitbox.BottomRight.Y || hitbox.TopLeft.Y < BottomRight.Y)
            {
                return false;
            }

            //now the rectangles must be intersecting
            return true;

        }

        public override bool Equals(object obj)
        {
            try
            {
                if (obj ! is Hitbox) return false;
            }
            catch (Exception)
            {
                // ignored
            }

            var hb = (Hitbox) obj;

            return hb.TopLeft == this.TopLeft && hb.BottomRight == this.BottomRight;

        }

        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() ^ BottomRight.GetHashCode();
        }

        public static bool operator ==(Hitbox left, Hitbox right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Hitbox left, Hitbox right)
        {
            return !(left == right);
        }
    }
}