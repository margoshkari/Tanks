using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client
{
    public class Tank
    {
        public Texture2D texture { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Speed { get; set; }
        public int[] Color { get; set; }
        public float Rotation { get; set; }
        public Tank(Texture2D texture)
        {
            Random rand = new Random();
            this.texture = texture;
            CoordX = 300;
            CoordY = 300;
            Speed = 3;
            Color = new int[] { rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255) };
            Rotation = 0f;
        }
    }
}
