using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankDll;

namespace Client
{
    public class Sprite
    {
        public Texture2D tankTexture { get; set; }
        public Texture2D bulletTexture { get; set; }
        public Tank tank { get; set; }
        public Sprite(Texture2D tankTexture, Texture2D bulletTexture, Tank tank)
        {
            this.tankTexture = tankTexture;
            this.bulletTexture = bulletTexture;
            this.tank = tank;
        }
    }
}
