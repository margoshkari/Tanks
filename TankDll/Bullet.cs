using System;
using System.Collections.Generic;
using System.Text;

namespace TankDll
{
    public class Bullet
    {
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Speed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool isActive { get; set; }
        public float Rotation { get; set; }
        public Direction dir { get; set; }
        public Bullet()
        {
            Random rand = new Random();
            CoordX = -10;
            CoordY = -10;
            Speed = 10;
            Width = 20;
            Height = 20;
            isActive = false;
            Rotation = 0f;
            dir = Direction.Up;
        }
    }
}
