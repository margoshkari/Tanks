using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using WinFormsApp1;

namespace Client
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Menu menu;
        private Gameplay gameplay;
        private Rating rating;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Main.Start();
            menu = new Menu();
            rating = new Rating();
            gameplay = new Gameplay(Content);
            menu.LoadContent(Content);
            rating.LoadContent(Content);
            gameplay.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                gameplay.SaveData();
                Exit();
            }

            if (gameplay.isActive)
                gameplay.Update();
            else if (rating.isActive)
                rating.Update();
            else
                menu.Update();

            if (menu.playButton.isClick)
            {
                gameplay.isActive = true;
                menu.playButton.isClick = false;
            }
            if (menu.ratingButton.isClick)
            {
                rating.isActive = true;
                menu.ratingButton.isClick = false;
                rating.currentScore = gameplay.currentTank.tank.Score;
            }
            Window.Title = gameplay.currentTank.tank.ID.ToString();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            if (gameplay.isActive)
                gameplay.Draw(_spriteBatch);
            else if (rating.isActive)
                rating.Draw(_spriteBatch);
            else
                menu.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
