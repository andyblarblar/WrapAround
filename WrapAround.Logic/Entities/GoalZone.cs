using System;
using System.Numerics;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    [Serializable]
    public class GoalZone : IQuadrantHitbox
    {
        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public QuadrantController SegmentController { get; set; } 

        public GoalZone(Vector2 position)
        {
            SegmentController = new QuadrantController();
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 10, Position.Y + 703));
            _ = SegmentController.UpdateSegment(Hitbox);
        }


    }
}