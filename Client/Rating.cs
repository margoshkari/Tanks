using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client
{
    public class Rating
    {
        public bool isActive;
        private SpriteFont font;
        public Rating()
        {
            isActive = false;
    }
        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>(@"Font\font_20");
        }
        public void Update()
        {
            Exit();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(font, "Top Players:", new Vector2(300, 50), Color.White);
        }

        private void Exit()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                isActive = false;
            }
        }
    }
}
