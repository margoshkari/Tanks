using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Client
{
    public class Menu
    {
        public bool isActive;
        public Button playButton;
        public Button ratingButton;
        private Button exitButton;
        private SpriteFont font;
        public Menu()
        {
            isActive = true;
        }
        public void LoadContent(ContentManager content)
        {
            playButton = new Button(content.Load<Texture2D>(@"Textures\playBtn"), 300, 150);
            ratingButton = new Button(content.Load<Texture2D>(@"Textures\playBtn"), playButton.CoordX, playButton.CoordY + playButton.Height + 10);
            exitButton = new Button(content.Load<Texture2D>(@"Textures\playBtn"), ratingButton.CoordX, ratingButton.CoordY + ratingButton.Height + 10);
            font = content.Load<SpriteFont>(@"Font\font_20");
        }
        public void Update()
        {
            Hover(playButton);
            Hover(ratingButton);
            Hover(exitButton);

            if (exitButton.isClick)
                Environment.Exit(0);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(playButton.texture, new Rectangle(playButton.CoordX, playButton.CoordY, playButton.Width, playButton.Height), playButton.color);
            _spriteBatch.Draw(ratingButton.texture, new Rectangle(ratingButton.CoordX, ratingButton.CoordY, ratingButton.Width, ratingButton.Height), ratingButton.color);
            _spriteBatch.Draw(exitButton.texture, new Rectangle(exitButton.CoordX, exitButton.CoordY, exitButton.Width, exitButton.Height), exitButton.color);

            _spriteBatch.DrawString(font, "Play", new Vector2(playButton.CoordX + 50, playButton.CoordY + 10), Color.White);
            _spriteBatch.DrawString(font, "Rating", new Vector2(ratingButton.CoordX + 40, ratingButton.CoordY + 10), Color.White);
            _spriteBatch.DrawString(font, "Exit", new Vector2(exitButton.CoordX + 50, exitButton.CoordY + 10), Color.White);
        }

        public void Hover(Button button)
        {
            var mouse = Mouse.GetState();
            var mousePosition = new Point(mouse.X, mouse.Y);
            Rectangle btn = new Rectangle(button.CoordX, button.CoordY, button.Width, button.Height);

            if (btn.Intersects(new Rectangle(mousePosition.X, mousePosition.Y, 20, 20)))
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
