using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Client
{
    public class Rating
    {
        public bool isActive;
        private SpriteFont font;
        public int currentScore;
        public Rating()
        {
            isActive = false; 
            currentScore = 0;
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
            if(File.Exists(@$"C:\ProgramData\Tanks\rating.txt"))
            {
                _spriteBatch.DrawString(font, File.ReadAllText(@$"C:\ProgramData\Tanks\rating.txt"), new Vector2(300, 90), Color.White);
            }
            _spriteBatch.DrawString(font, $"Your score: {currentScore}", new Vector2(300, 400), Color.White);
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
