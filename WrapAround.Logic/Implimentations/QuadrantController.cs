using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Implimentations
{
    /// <summary>
    /// An object that contains the segment state of the surrounding object
    /// </summary>
    public class QuadrantController : IGameMapSegmentController<Quadrant>
    {
        /// <summary>
        /// A list of all quad rents one is in
        /// </summary>
        public List<Quadrant> Segment { get; set; }

        public (int, int) CanvasSize { get; set; }

        /// <summary>
        /// Updates Segment to the current Quadrant(s)
        /// </summary>
        /// <param name="position">current position of the object</param>
        public async Task UpdateSegment(Hitbox hitbox)
        {
            await Task.Run(() =>
            {
                Segment.Clear();

                var (canX, canY) = CanvasSize;

                var centerCorrs = new Vector2(canX * .5f, canY * .5f);

                var resultantL = centerCorrs + hitbox.TopLeft;
                var resultantR = centerCorrs + hitbox.BottomRight;

                var leftSeg = (centerCorrs,resultantL) switch
                {
                    var (center, res) when res.X > center.X && res.Y >= center.Y => Quadrant.Quadrent1,
                    var (center, res) when res.X < center.X && res.Y >= center.Y => Quadrant.Quadrent2,
                    var (center, res) when res.X < center.X && res.Y <= center.Y => Quadrant.Quadrent3,
                    var (center, res) when res.X > center.X && res.Y <= center.Y => Quadrant.Quadrent4
                };

                var rightSeg = (centerCorrs, resultantR) switch
                {
                    var (center, res) when res.X > center.X && res.Y >= center.Y => Quadrant.Quadrent1,
                    var (center, res) when res.X < center.X && res.Y >= center.Y => Quadrant.Quadrent2,
                    var (center, res) when res.X < center.X && res.Y <= center.Y => Quadrant.Quadrent3,
                    var (center, res) when res.X > center.X && res.Y <= center.Y => Quadrant.Quadrent4
                };

                Segment.Add(leftSeg);
                Segment.Add(rightSeg);

            });

        }








    }
}