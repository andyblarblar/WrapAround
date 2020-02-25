
namespace WrapAround.Logic.Entities
{
    public class ScoreBoard
    {
        public (int, int) Score { get; set; }

        public void Reset()
        {
            Score = (0, 0);
        }

        public void ScoreLeft()
        {
            var (leftScore, rightScore) = Score;

            Score = (++leftScore, rightScore);
        }

        public void ScoreRight()
        {
            var (leftScore, rightScore) = Score;

            Score = (leftScore, ++rightScore);
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
