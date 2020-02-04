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

        /// <summary>
        /// Serializes this map object to a standard model.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a GameMap from a serialized string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static GameMap DeSerialize(string input)
        {
            throw new NotImplementedException();
        }

    }
}