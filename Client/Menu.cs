using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client
{
    public class Menu
    {
        public bool isActive = true;
        public Button playButton;
        private SpriteFont font;
        public Menu()
        {
        }
        public void LoadContent(ContentManager content)
        {
            playButton = new Button(content.Load<Texture2D>(@"Textures\playBtn"));
            font = content.Load<SpriteFont>(@"Font\font_20");
        }
        public void Update()
        {
            Hover(playButton);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(playButton.texture, new Rectangle(playButton.CoordX, playButton.CoordY, playButton.Width, playButton.Height), playButton.color);
            _spriteBatch.DrawString(font, "Play", new Vector2(playButton.CoordX + 50, playButton.CoordY + 10), Color.White);
        }

        public void Hover(Button button)
        {
            var mouse = Mouse.GetState();
            var mousePosition = new Point(mouse.X, mouse.Y);
            Rectangle btn = new Rectangle(button.CoordX, button.CoordY, button.Width, button.Height);

            if (btn.Intersects(new Rectangle(mousePosition.X, mousePosition.Y, 30, 30)))
            {
                button.color = Color.Blue;
                Click(button);
            }
            else
                button.color = Color.BlueViolet;
        }
        public void Click(Button button)
        {
            var mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                button.isClick = true;
            }
        }
    }
}
