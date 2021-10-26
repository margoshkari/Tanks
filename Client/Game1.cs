using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TankDll;


namespace Client
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ClientData clientData;
        private List<Tank> tanks;
        private List<Sprite> tankSprites;
        private Sprite currentTank;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            clientData = new ClientData();
            tanks = new List<Tank>();
            tankSprites = new List<Sprite>();
        }

        protected override void Initialize()
        {
            clientData.socket.Connect(clientData.iPEndPoint);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            currentTank = new Sprite(Content.Load<Texture2D>(@"Textures\tank"), new Tank());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            string json = string.Empty;
            tankSprites.Clear();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                currentTank.tank.CoordY -= currentTank.tank.Speed;
                currentTank.tank.Rotation = 0f;
               
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                currentTank.tank.CoordY += currentTank.tank.Speed;
                currentTank.tank.Rotation = 15.7f;
                
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                currentTank.tank.CoordX -= currentTank.tank.Speed;
                currentTank.tank.Rotation = -7.85f;
               
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                currentTank.tank.CoordX += currentTank.tank.Speed;
                currentTank.tank.Rotation = 7.85f;
               
            }

            SendMsg();
            GetMsg();

            foreach (var item in tanks)
            {
                tankSprites.Add(new Sprite(Content.Load<Texture2D>(@"Textures\tank"), item));
            }

            base.Update(gameTime);
        }
        private void GetMsg()
        {
            try
            {
                string json = string.Empty;
                json = clientData.GetMsg();
                tanks = JsonSerializer.Deserialize<List<Tank>>(json);
            }
            catch (Exception ex) { }
        }
        private void SendMsg()
        {
            string json = string.Empty;
            json = JsonSerializer.Serialize<Tank>(currentTank.tank);
            clientData.socket.Send(Encoding.Unicode.GetBytes(json));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var item in tankSprites)
            {
                _spriteBatch.Draw(item.texture, new Rectangle(item.tank.CoordX, item.tank.CoordY, item.texture.Width, item.texture.Height), null, Color.White, item.tank.Rotation, new Vector2(item.texture.Width / 2f, item.texture.Height / 2f), SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
