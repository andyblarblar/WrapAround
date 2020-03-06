
using System.Numerics;
using System.Text.Json.Serialization;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    public class ScoreBoard
    {
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 Score { get; set; }

        public void Reset()
        {
            Score = new Vector2(0,0);
        }

        public void ScoreLeft()
        {
            var (leftScore, rightScore) = Score;

            Score = new Vector2(++leftScore, rightScore);
        }

        public void ScoreRight()
        {
            var (leftScore, rightScore) = Score;

            Score = new Vector2(leftScore, ++rightScore);
        }


        /// <summary>
        /// Finds if the game is won
        /// </summary>
        /// <returns>A tuple of (bool,bool) where the side of the bool corresponds to the side of the player.</returns>
        public (bool, bool) IsWon()
        {
            return Score switch
            {
                var (left, _) when left >= 10 => (true, false),
                var (_, right) when right >= 10 => (false, true),
                _ => (false, false)

            };

        }

    }

}
