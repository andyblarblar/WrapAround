using System;
using System.Collections.Generic;
using System.Text;

namespace WrapAround.Logic
{
    /// <summary>
    /// Represents an entity that can be collided with. 
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// Logic that defines collisions.
        /// </summary>
        /// <param name="collided">The other entity that was collided</param>
        void Collide(object collided);
        

    }





}
