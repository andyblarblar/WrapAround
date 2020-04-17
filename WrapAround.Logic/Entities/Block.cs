using System;
using System.Drawing;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;
using MessagePack;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 40 px by 20 px breakable breakout-esk block
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class Block : IDestructable, IQuadrantHitbox, ICollidable, IEquatable<Block>
    {
        [Key("health")]
        public int health { get; set; }

        [Key("hitbox")]
        public Hitbox Hitbox { get; set; }

        [JsonIgnore]
        [IgnoreMember]
        public QuadrantController SegmentController { get; set; }

        [Key("color")]
        [JsonIgnore]
        public string Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">The top left position of the block</param>
        public Block(Vector2 position)
        {
            SegmentController = new QuadrantController();

            health = 10;
            Hitbox = new Hitbox(position, new Vector2(position.X + 40, position.Y + 20));

            //initialise position
            _ = SegmentController.UpdateSegment(Hitbox);
        }

        /// <summary>
        /// For serialization support
        /// </summary>
        public Block()
        {
            Color = "rgb(153,217,234)";//change to 5,5,5 for invisible mode
            SegmentController = new QuadrantController();

            //initialise position
            _ = SegmentController.UpdateSegment(Hitbox);
        }

        /// <summary>
        /// For MessagePack
        /// </summary>
        [SerializationConstructor]
        public Block(int health, Hitbox hitbox, string color)
        {
            this.health = health;
            Hitbox = hitbox;
            Color = color;
        }

        /// <summary>
        /// Decrements the blocks health and color
        /// </summary>
        public virtual void Damage()
        {
            health--;

            Color = health switch
            {
                2 => "rgb(240,255,0)",
                4 => "rgb(132,255,0)",
                6 => "rgb(0,255,156)",
                8 => "rgb(53,205,255)",
                10 => "rgb(153,217,234)",
                _ => "rgb(153,217,234)"
            };

        }


        public async Task Collide(object _)
        {
            await Task.Run(Damage);
        }


        public void Reset()
        {
            health = 5;
            Color = "rgb(255,255,255)";
        }

        public bool Equals(Block other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return health == other.health && Hitbox.Equals(other.Hitbox);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Block) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(health, Hitbox);
        }

        public static bool operator ==(Block left, Block right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Block left, Block right)
        {
            return !Equals(left, right);
        }
    }
}