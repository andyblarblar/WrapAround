using System.Numerics;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    public class GoalZone : IQuadrantHitbox
    {
        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public QuadrantController SegmentController { get; set; } = new QuadrantController();

        public GoalZone(Vector2 position)
        {
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10,Position.Y + 703));

        }


    }
}