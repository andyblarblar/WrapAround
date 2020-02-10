using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using WrapAround.Logic.Interfaces;

namespace WrapAround.Logic.Entities
{
    public class Block : IDestructable
    {
        public int health { get; set; }

        public Vector2 Position { get; set; }

        public Color Color { get; set; }


        /// <summary>
        /// Decrements the blocks health and color
        /// </summary>
        public void Damage()
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