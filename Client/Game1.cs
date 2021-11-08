using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using TankDll;

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
                Exit();

            if (gameplay.isActive)
                gameplay.Update();
            else if (rating.isActive)
                rating.Update();
            else
                menu.Update();

            if (menu.playButton.isClick)
            {
                menu.playButton.isClick = false;
                gameplay.isActive = true;
            }
            if (menu.ratingButton.isClick)
            {
                menu.ratingButton.isClick = false;
                rating.isActive = true;
            }
            Window.Title = gameplay.currentTank.tank.Score.ToString();

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
