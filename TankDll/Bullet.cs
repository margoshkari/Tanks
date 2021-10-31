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
        public Bullet()
        {
            Random rand = new Random();
            CoordX = -10;
            CoordY = -10;
            Speed = 10;
        }
    }
}
