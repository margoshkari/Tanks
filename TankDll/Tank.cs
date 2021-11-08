using System;

namespace TankDll
{
    public class Tank
    {
        public int Score { get; set; }
        public int HP { get; set; }
        public int Damage { get; set; }
        public int ID { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Speed { get; set; }
        public int[] Color { get; set; }
        public float Rotation { get; set; }
        public Bullet bullet { get; set; }
        public Direction dir { get; set; }
        public Tank()
        {
            Random rand = new Random();
            bullet = new Bullet();
            CoordX = 100;
            CoordY = 100;
            Speed = 3;
            Color = new int[] { rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255) };
            Rotation = 0f;
            HP = 100;
            Damage = 20;
            Score = 0;
            dir = Direction.Up;
        }
    }
}
