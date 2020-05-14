using System;
using System.Numerics;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    public class GoalZone
    {
        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public GoalZone(Vector2 position)
        {
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + 703));
        }

        public bool IsCollidingWith(in Hitbox hitbox) => Hitbox.IsCollidingWith(in hitbox);


    }
}