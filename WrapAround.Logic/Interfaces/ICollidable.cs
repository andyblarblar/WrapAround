using System.Threading.Tasks;

namespace WrapAround.Logic.Interfaces
{
    /// <summary>
    /// Represents an entity that can be collided with. 
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// Logic that defines collisions.
        /// </summary>
        void Collide<T>(in T collidedWith);



    }





}
