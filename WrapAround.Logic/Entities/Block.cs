using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using WrapAround.Logic.Util;
using MessagePack;

namespace WrapAround.Logic.Entities
{
    /// <summary>
    /// A 40 px by 20 px breakable breakout-esk block
    /// </summary>
    [MessagePackObject]
    public readonly struct Block : IEquatable<Block>
    {
        [Key("health")] public int Health { get; }

        [Key("hitbox")] public Hitbox Hitbox { get; }

        [Key("color")] public string Color { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">The top left position of the block</param>
        public Block(Vector2 position)
        {
            Health = 5;
            Hitbox = new Hitbox(position, new Vector2(position.X + 40, position.Y + 20));
            Color = GetColorFromHealth(Health);
        }

        [SerializationConstructor]
        [JsonConstructor]
        public Block(int health, Hitbox hitbox, string color)
        {
            Health = health;
            Hitbox = hitbox;
            Color = color;
        }

        public static string GetColorFromHealth(int health)
        {
            return health switch
            {
                1 => "rgb(240,255,0)",
                2 => "rgb(132,255,0)",
                3 => "rgb(0,255,156)",
                4 => "rgb(53,205,255)",
                5 => "rgb(153,217,234)",
                _ => "rgb(153,217,234)"
            };

        }

        public bool IsCollidingWith(in Hitbox hitbox) => Hitbox.IsCollidingWith(in hitbox);

        /// <summary>
        /// Takes a block and returns a new block with reset health and color.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static Block Reset(in Block block)
        {
            return new Block(block.Hitbox.TopLeft);
        }

        /// <summary>
        /// Takes a block, calculates damaging, and returns the new damaged block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static Block DamageBlock(in Block block)
        {
            return new Block(health: block.Health - 1, hitbox: block.Hitbox, color: GetColorFromHealth(block.Health - 1));
        }

        public bool Equals(Block other)
        {
            return Health == other.Health && Hitbox.Equals(other.Hitbox) && Color == other.Color;
        }

        public override bool Equals(object obj)
        {
            return obj is Block other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Health, Hitbox, Color);
        }

        public static bool operator ==(Block left, Block right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Block left, Block right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Health:{Health},Hitbox:{Hitbox}";
        }
    }
}