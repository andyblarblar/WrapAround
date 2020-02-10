using System.Numerics;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    public class GoalZone : IHitbox
    {
        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public GoalZone(Vector2 position)
        {
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10,Position.Y + 703));

        }


    }
}