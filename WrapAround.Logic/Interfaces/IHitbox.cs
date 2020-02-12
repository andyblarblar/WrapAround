using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Interfaces
{
    /// <summary>
    /// Gives the ability to Segment the implementer and supply a hitbox for collision detection
    /// </summary>
    /// <typeparam name="T">The Type of the segment controller</typeparam>
    /// <typeparam name="TU">the type of the segments</typeparam>
    public interface IHitbox<T, TU> where T : IGameMapSegmentController<TU>
    {
        Hitbox Hitbox { get; set; }

        T SegmentController { get; set; }

    }

    /// <summary>
    /// Represents a nondescript segment of a game map
    /// <typeparam name="T">The Segment Type</typeparam>
    /// </summary>
    public interface IGameMapSegmentController<T>
    {
        /// <summary>
        /// The type that will designate the segments
        /// </summary>
        List<T> Segment { get; }

        (int, int) CanvasSize { get; set; }

        Task UpdateSegment(Hitbox hitbox);


    }
}