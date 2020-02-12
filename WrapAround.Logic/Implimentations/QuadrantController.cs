using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Implimentations
{
    /// <summary>
    /// An object that contains the segment state of the surrounding object
    /// </summary>
    public class QuadrantController : IGameMapSegmentController<Quadrant>
    {
        public Quadrant Segment { get; set; }

        public (int, int) CanvasSize { get; set; }

        /// <summary>
        /// Updates Segment to the current Quadrant
        /// </summary>
        /// <param name="position">current position of the object</param>
        public async Task UpdateSegment(Vector2 position)
        {//TODO update to run on both sides of the hitbox and make it so you can be in multiple segments at once
            await Task.Run(() =>
            {
                var (canX, canY) = CanvasSize;

                var centerCorrs = new Vector2(canX * .5f, canY * .5f);

                var resultant = centerCorrs + position;

                Segment = Math.Atan(resultant.Y / resultant.X) switch
                {
                    var r when r >= 0 && r <= 90 => Quadrant.Quadrent1, //90 and 0 degrees count as quadrant 1
                    var r when r > 90 && r <= 180 => Quadrant.Quadrent2, //180 is in 2
                    var r when r > 180 && r <= 270 => Quadrant.Quadrent3, //270 is in 3
                    var r when r > 270 && r <= 360 => Quadrant.Quadrent4 //360 is in 4
                };

            });

        }








    }
}