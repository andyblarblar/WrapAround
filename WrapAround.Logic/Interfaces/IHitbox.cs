using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WrapAround.Logic.Interfaces
{
    public interface IHitbox
    {

        Hitbox Hitbox { get; set; }

    }

    /// <summary>
    /// A rectangle representing a hitbox
    /// </summary>
    public struct Hitbox
    {
        public Point TopLeft, BottomRight;

        public Hitbox(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        /// <summary>
        /// Checks if two rectangles are intersecting by checking if either Rectangle is above
        /// or to the left of the other rectangle.
        /// </summary>
        /// <param name="hitbox">other hitbox</param>
        public bool IsCollidingWith(Hitbox hitbox)
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


    }





}
