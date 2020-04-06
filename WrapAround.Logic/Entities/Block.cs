using System;
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
    [Serializable]
    public class Block : IDestructable, IQuadrantHitbox, ICollidable
    {
        public int health { get; set; }

        public Hitbox Hitbox { get; set; }

        [JsonIgnore]
        public QuadrantController SegmentController { get; set; } 

        public Color Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">The top left position of the block</param>
        public Block(Vector2 position)
        {
            SegmentController = new QuadrantController();

            health = 5;
            Hitbox = new Hitbox(position, new Vector2(position.X + 40, position.Y + 20));

            //initialise position
            _ = SegmentController.UpdateSegment(Hitbox);
        }

        /// <summary>
        /// For serialization support
        /// </summary>
        public Block()
        {
            Color = Color.Azure;

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