using System;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Implimentations
{
    /// <summary>
    /// Provides utility for a class to be indexed into quadrants, for use in collision detection
    /// </summary>
    [Obsolete("Quadrant segments are no longer supported")]
    public interface IQuadrantHitbox : IHitbox<QuadrantController, Quadrant>
    {

    }


}
