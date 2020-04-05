using System.Collections.Generic;
using System.Numerics;
using WrapAround.Logic.Entities;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents the layout of a map.
    /// </summary>
    public class GameMap
    {

        public List<Block> Blocks { get; }
        public (int, int) CanvasSize { get; set; } = (1250, 703);
        public GoalZone LeftGoal;
        public GoalZone RightGoal;

        public GameMap(List<Block> blocks = default)
        {
            if (blocks == null)
            {
                blocks = new List<Block> {new Block(new Vector2(500, 300))};
            }

            LeftGoal = new GoalZone(new Vector2(0, 0));
            RightGoal = new GoalZone(new Vector2(CanvasSize.Item1 - 20, 0));
            Blocks = blocks;
        }

    }
}