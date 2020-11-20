using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using WrapAround.Logic.Entities;
using MessagePack;
using WrapAround.Logic.Util;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents the layout of a map.
    /// </summary>
    [MessagePackObject]
    public class GameMap
    {
        [Key("blocks")]
        public Block[] Blocks { get; set; }

        [JsonIgnore]
        [IgnoreMember]
        public (int, int) CanvasSize { get; set; } = (1250, 703);

        [IgnoreMember]
        private readonly GoalZone _leftGoal;

        [IgnoreMember]
        private readonly GoalZone _rightGoal;

        /// <summary>
        /// For serialization support
        /// </summary>
        public GameMap()
        {
            Blocks ??= Array.Empty<Block>();

            CanvasSize = (1250, 703);
            _leftGoal = new GoalZone(new Vector2(0, 0));
            _rightGoal = new GoalZone(new Vector2(CanvasSize.Item1 - 20, 0));
        }

        public GameMap(Block[] blocks = default)
        {
            blocks ??= Array.Empty<Block>();

            _leftGoal = new GoalZone(new Vector2(0, 0));
            _rightGoal = new GoalZone(new Vector2(CanvasSize.Item1 - 20, 0));
            Blocks = blocks;
        }

        /// <summary>
        /// Checks if the hitbox is in the goal
        /// </summary>
        /// <returns>A bool tuple (l,r), where either are true if a goal has been scored in the respective sides goal</returns>
        public (bool,bool) CheckForGoal(in Hitbox hitbox)
        {
            //Goal scoring
            if (_leftGoal.IsCollidingWith(in hitbox))
            {
                return (true, false);
            }

            else if (_rightGoal.IsCollidingWith(in hitbox))
            {
                return (false, true);
            }

            return (false, false);

        }


        public void Reset()
        {
            //reset health of all blocks
            for (var i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = Block.Reset(in Blocks[i]);
            }
        }

    }
}