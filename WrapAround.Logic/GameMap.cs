using System;
using System.Collections.Generic;
using WrapAround.Logic.Entities;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents the layout of a map.
    /// </summary>
    public class GameMap
    {
       
        private List<Block> blocks;
        public Tuple<int, int> canvasSize { get; set; }
        private GoalZone leftGoal;
        private GoalZone rightGoal;

        public GameMap((int x, int y) canvasSize, List<Block> blocks)
        {
            //TODO create GoalZone as an offset of canvas size

        }

    }
}