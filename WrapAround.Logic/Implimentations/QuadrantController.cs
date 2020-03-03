using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
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
        /// A list of all quadrants one is in
        /// </summary>
        public List<Quadrant> Segment { get; } = new List<Quadrant>();

        public (int, int) CanvasSize { get; set; }

        /// <summary>
        /// Updates Segment to the current Quadrant(s)
        /// </summary>
        public async Task UpdateSegment(Hitbox hitbox)
        {
            await Task.Run(() =>
            {
                Segment.Clear();

                var (canX, canY) = CanvasSize;

                var centerCorrs = new Vector2(canX * .5f, canY * .5f);

                var resultantL = centerCorrs + hitbox.TopLeft;
                var resultantR = centerCorrs + hitbox.BottomRight;


                var leftSeg = (centerCorrs, resultantL) switch
                {
                    var (center, res) when res.X >= center.X && res.Y >= center.Y => Quadrant.Quadrant1,
                    var (center, res) when res.X <= center.X && res.Y >= center.Y => Quadrant.Quadrant2,
                    var (center, res) when res.X <= center.X && res.Y <= center.Y => Quadrant.Quadrant3,
                    var (center, res) when res.X >= center.X && res.Y <= center.Y => Quadrant.Quadrant4,
                    _ => throw new Exception("Failed to find segment")
                };

                var rightSeg = (centerCorrs, resultantR) switch
                {
                    var (center, res) when res.X >= center.X && res.Y >= center.Y => Quadrant.Quadrant1,
                    var (center, res) when res.X <= center.X && res.Y >= center.Y => Quadrant.Quadrant2,
                    var (center, res) when res.X <= center.X && res.Y <= center.Y => Quadrant.Quadrant3,
                    var (center, res) when res.X >= center.X && res.Y <= center.Y => Quadrant.Quadrant4,
                    _ => throw new Exception("Failed to find segment")
                };

                Segment.Add(leftSeg);
                Segment.Add(rightSeg);

            });

        }








    }
}