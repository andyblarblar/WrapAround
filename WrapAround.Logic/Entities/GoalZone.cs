using System.Numerics;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// Represents the area that counts as a goal. 
    /// </summary>
    public class GoalZone
    {
        public Vector2 Position { get; set; }

        public (int, int) WidthHigthTuple { get; } = (10, 703);

    }
}