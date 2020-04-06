using System;
using System.Collections.Generic;
using System.Numerics;
using WrapAround.Logic.Entities;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents the layout of a map.
    /// </summary>
    [Serializable]
    public class GameMap
    {
        public List<Block> Blocks { get; set; }
        public (int, int) CanvasSize { get; set; } = (1250, 703);
        public GoalZone LeftGoal;
        public GoalZone RightGoal;

        /// <summary>
        /// For serialization support
        /// </summary>
        public GameMap()
        {
            CanvasSize = (1250, 703);
            LeftGoal = new GoalZone(new Vector2(0, 0));
            RightGoal = new GoalZone(new Vector2(CanvasSize.Item1 - 20, 0));
        }

        public GameMap(List<Block> blocks = default)
        {
            if (blocks == null)
            {
                blocks = new List<Block>();
            }

            LeftGoal = new GoalZone(new Vector2(0, 0));
            RightGoal = new GoalZone(new Vector2(CanvasSize.Item1 - 20, 0));
            Blocks = blocks;
        }

    }
}