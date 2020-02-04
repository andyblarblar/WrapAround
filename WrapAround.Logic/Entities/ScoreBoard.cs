using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace WrapAround.Logic.Entities
{
    class ScoreBoard
    {
        public (int, int) score { get; set; } = (0, 0);

        public void Reset()
        {
            score = (0, 0);
        }




    }
}
