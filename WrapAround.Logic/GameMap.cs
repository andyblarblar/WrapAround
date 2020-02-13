﻿using System;
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

        public List<Block> Blocks { get; set; } 
        public (int, int) CanvasSize { get; set; } = (1250, 703);
        public GoalZone LeftGoal;
        public GoalZone RightGoal;

        public GameMap(List<Block> blocks = default, (int x, int y) canvasSize = default)
        {
            if (blocks is null) blocks = new List<Block>();
            
            LeftGoal = new GoalZone(new Vector2(0, 0));
            RightGoal = new GoalZone(new Vector2(canvasSize.x - 10, 0));
            Blocks = blocks;
        }

    }
}