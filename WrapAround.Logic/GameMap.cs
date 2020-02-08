using System;
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
       
        public List<Block> blocks { get; set; } = new List<Block>();
        public (int, int) canvasSize { get; set; } = (1250, 703);
        public GoalZone leftGoal; 
        public GoalZone rightGoal;

        public GameMap((int x, int y) canvasSize = default, List<Block> blocks = default)
        {
            leftGoal = new GoalZone{Position = new Vector2(0,0)};
            rightGoal = new GoalZone{Position = new Vector2(canvasSize.x - 10,0)};
        }

    }
}