using System.Drawing;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 40 px by 20 px breakable breakout-esk block
    /// </summary>
    public class Block : IDestructable, IQuadrantHitbox, ICollidable
    {
        public int health { get; set; }

        [JsonIgnore]
        public Vector2 Position { get; set; }

        public Hitbox Hitbox { get; set; }

        public QuadrantController SegmentController { get; set; } 

        public Color Color { get; set; }

        public Block(Vector2 position)
        {
            SegmentController = new QuadrantController();

            health = 5;
            Position = position;
            Hitbox = new Hitbox(Position, new Vector2(Position.X + 40, Position.Y + 20));

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
                5 => Color.Azure,
                _ => Color.Empty
            };

        }


        public async Task Collide(object _)
        {
            await Task.Run(Damage);
        }
    }
}