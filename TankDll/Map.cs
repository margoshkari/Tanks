

namespace TankDll
{
    public class Map
    {
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsWall { get; set; }
        public Map(int coordX, int coordY, bool isWall)
        {
            CoordX = coordX;
            CoordY = coordY;
            Width = 40;
            Height = 40;
            IsWall = isWall;
        }
    }
}
