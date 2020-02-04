using System.Numerics;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    public class Block : IDestructable
    {
        public int health { get; set; }

        public Vector2 position { get; set; }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}