using System;

namespace Client
{
    public class Tank
    {
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Speed { get; set; }
        public int[] Color { get; set; }
        public float Rotation { get; set; }
        public Tank()
        {
            Random rand = new Random();
            CoordX = 300;
            CoordY = 300;
            Speed = 3;
            Color = new int[] { rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255) };
            Rotation = 0f;
        }
    }
}
