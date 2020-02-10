using System.Drawing;
using System.Numerics;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 40 px by 20 px breakable breakout-esk block
    /// </summary>
    public class Block : IDestructable, IHitbox
    {
        public int health { get; set; }

        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public Color Color { get; set; }

        public Block(Vector2 position)
        {
            health = 5;
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 40,Position.Y + 20));

        }

        /// <summary>
        /// Decrements the blocks health and color
        /// </summary>
        public virtual void Damage()
        {
            health--;

            Color = health switch
            {
                1 => Color.Red,
                2 => Color.Orange,
                3 => Color.Yellow,
                4 => Color.Green,
                5 => Color.Azure
            };

        }


        
    }
}