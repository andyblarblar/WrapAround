using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using WrapAround.Logic.Entities;
using MessagePack;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents the layout of a map.
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class GameMap
    {
        [Key("blocks")]
        public List<Block> Blocks { get; set; }

        [JsonIgnore]
        [IgnoreMember]
        public (int, int) CanvasSize { get; set; } = (1250, 703);

        [IgnoreMember]
        public GoalZone LeftGoal;

        [IgnoreMember]
        public GoalZone RightGoal;

        /// <summary>
        /// For serialization support
        /// </summary>
        public GameMap()
        {
            if (Blocks == null)
            {
                Blocks = new List<Block>();
            }

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

        public void Reset()
        {
            //reset health of all blocks
            Blocks.ForEach(block => block.Reset());
        }

    }
}