using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Client
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ClientData clientData;
        private List<Tank> tankSprites;
        private Tank currentTank;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            clientData = new ClientData();
            tankSprites = new List<Tank>();
        }

        protected override void Initialize()
        {
            clientData.socket.Connect(clientData.iPEndPoint);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            currentTank = new Tank(Content.Load<Texture2D>(@"Textures\tank"));
            tankSprites.Add(currentTank);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                currentTank.CoordY -= currentTank.Speed;
                currentTank.Rotation = 0f;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.S))
            {
                currentTank.CoordY += currentTank.Speed;
                currentTank.Rotation = 15.7f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                currentTank.CoordX -= currentTank.Speed;
                currentTank.Rotation = -7.85f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                currentTank.CoordX += currentTank.Speed;
                currentTank.Rotation = 7.85f;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var item in tankSprites)
            {
                _spriteBatch.Draw(item.texture, new Rectangle(item.CoordX, item.CoordY, item.texture.Width, item.texture.Height), null, Color.White, item.Rotation, new Vector2(item.texture.Width / 2f, item.texture.Height / 2f), SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
