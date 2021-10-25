using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankDll;

namespace Client
{
    public class Sprite
    {
        public Texture2D texture { get; set; }
        public Tank tank { get; set; }
        public Sprite(Texture2D texture, Tank tank)
        {
            this.texture = texture;
            this.tank = tank;
        }
    }
}
