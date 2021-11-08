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
            gameplay = new Gameplay(Content);
            menu.LoadContent(Content);
            gameplay.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (menu.isActive)
                menu.Update();
            if (gameplay.isActive)
                gameplay.Update();

            if (menu.playButton.isClick)
            {
                menu.isActive = false;
                gameplay.isActive = true;
                Window.Title = gameplay.currentTank.tank.ID.ToString();
            }
            

            base.Update(gameTime);
        }
       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if(menu.isActive)
                menu.Draw(_spriteBatch);
            if (gameplay.isActive)
                gameplay.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
