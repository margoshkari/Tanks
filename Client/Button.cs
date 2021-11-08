
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    public class Button
    {
        public Texture2D texture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public Color color { get; set; }
        public bool isClick { get; set; }
        public Button(Texture2D texture)
        {
            Width = 150;
            Height = 50;
            CoordX = 300;
            CoordY = 200;
            isClick = false;
            color = Color.BlueViolet;
            this.texture = texture;
        }
    }
}
