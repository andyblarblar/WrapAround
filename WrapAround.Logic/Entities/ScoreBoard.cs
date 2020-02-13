using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace WrapAround.Logic.Entities
{
    
    class ScoreBoard
    {
        private (int, int) score;

        public void Reset()
        {
            score = (0, 0);
        }

        public void ScoreLeft()
        {
            score = (score.Item1++, score.Item2);
        }

        public void ScoreRight()
        {
            score = (score.Item1, score.Item2++);
        }

        public (int, int) GetScore => score;

        /// <summary>
        /// Finds if the game is won
        /// </summary>
        /// <returns>A tuple of (bool,bool) where the side of the bool corresponds to the side of the player.</returns>
        public (bool, bool) IsWon()
        {
            return score switch
            {
                var (left, _) when left >= 10 => (true, false),
                var (_, right) when right >= 10 => (false, true),
                _ => (false, false)

            };

        }



    }
}
