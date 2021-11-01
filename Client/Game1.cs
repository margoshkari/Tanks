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
        private Keys keys = Keys.W;
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
            currentTank = new Sprite(Content.Load<Texture2D>(@"Textures\tank"), Content.Load<Texture2D>(@"Textures\bullet"), new Tank());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            string json = string.Empty;
            tankSprites.Clear();

            SendMsg();
            GetMsg();

            foreach (var item in tanks)
            {
                tankSprites.Add(new Sprite(Content.Load<Texture2D>(@"Textures\tank"), Content.Load<Texture2D>(@"Textures\bullet"), item));
            }

            TankMove();
            BulletMove();

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
                _spriteBatch.Draw(item.bulletTexture, new Rectangle(item.tank.bullet.CoordX, item.tank.bullet.CoordY, item.tank.bullet.Width, item.tank.bullet.Height), null, Color.White, item.tank.bullet.Rotation, new Vector2(item.bulletTexture.Width / 2f, item.bulletTexture.Height / 2f), SpriteEffects.None, 0f);
                _spriteBatch.Draw(item.tankTexture, new Rectangle(item.tank.CoordX, item.tank.CoordY, item.tankTexture.Width, item.tankTexture.Height), null, new Color(item.tank.Color[0], item.tank.Color[1], item.tank.Color[2]), item.tank.Rotation, new Vector2(item.tankTexture.Width / 2f, item.tankTexture.Height / 2f), SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private bool TankCollision(Rectangle rect)
        {
            foreach (var item in tankSprites)
            {
                if (item.tank.CoordX != currentTank.tank.CoordX && item.tank.CoordY != currentTank.tank.CoordY)
                {
                    if (rect.Intersects(new Rectangle(item.tank.CoordX, item.tank.CoordY, item.tankTexture.Width, item.tankTexture.Height)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    
        private void TankMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (currentTank.tank.CoordY - (currentTank.tank.Speed + currentTank.tankTexture.Height / 2) > 0)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY - currentTank.tank.Speed,
                        currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordY -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = 0f;
                        if(!currentTank.tank.bullet.isActive)
                            keys = Keys.W;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (currentTank.tank.CoordY + (currentTank.tank.Speed + currentTank.tankTexture.Height / 2) < _graphics.PreferredBackBufferHeight)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY + currentTank.tank.Speed,
                           currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordY += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 15.7f;
                        if (!currentTank.tank.bullet.isActive)
                            keys = Keys.S;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (currentTank.tank.CoordX - (currentTank.tank.Speed + currentTank.tankTexture.Height / 2) > 0)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX - currentTank.tank.Speed, currentTank.tank.CoordY,
                             currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordX -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = -7.85f;
                        if (!currentTank.tank.bullet.isActive)
                            keys = Keys.A;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (currentTank.tank.CoordX + (currentTank.tank.Speed + currentTank.tankTexture.Height / 2) < _graphics.PreferredBackBufferWidth)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX + currentTank.tank.Speed, currentTank.tank.CoordY,
                                currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordX += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 7.85f;
                        if (!currentTank.tank.bullet.isActive)
                            keys = Keys.D;
                    }
                }
            }
        }
        private void BulletMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                currentTank.tank.bullet.CoordY = currentTank.tank.CoordY;
                currentTank.tank.bullet.CoordX = currentTank.tank.CoordX;
                currentTank.tank.bullet.Rotation = currentTank.tank.Rotation;
                currentTank.tank.bullet.isActive = true;
            }
            if(currentTank.tank.bullet.isActive)
            {
                if (keys == Keys.W)
                {
                    if(currentTank.tank.bullet.CoordY >= -10 && !BulletCollision())
                    {
                        currentTank.tank.bullet.CoordY -= currentTank.tank.bullet.Speed;
                    }
                    else
                        currentTank.tank.bullet.isActive = false;
                }
                else if (keys == Keys.S)
                {
                    if (currentTank.tank.bullet.CoordY <= _graphics.PreferredBackBufferHeight + 10 && !BulletCollision())
                    {
                        currentTank.tank.bullet.CoordY += currentTank.tank.bullet.Speed;
                    }
                    else
                        currentTank.tank.bullet.isActive = false;
                }
                else if (keys == Keys.A)
                {
                    if (currentTank.tank.bullet.CoordX >= -10 && !BulletCollision())
                    {
                        currentTank.tank.bullet.CoordX -= currentTank.tank.bullet.Speed;
                    }
                    else
                        currentTank.tank.bullet.isActive = false;
                }
                else if (keys == Keys.D)
                {
                    if (currentTank.tank.bullet.CoordX <= _graphics.PreferredBackBufferWidth + 10 && !BulletCollision())
                    {
                        currentTank.tank.bullet.CoordX += currentTank.tank.bullet.Speed;
                    }
                    else
                        currentTank.tank.bullet.isActive = false;
                }
            }
        }
        private bool BulletCollision()
        {
            Rectangle tank = new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY, currentTank.tankTexture.Width, currentTank.tankTexture.Height);

            foreach (var item in tankSprites)
            {
                if (item.tank.CoordX != currentTank.tank.CoordX && item.tank.CoordY != currentTank.tank.CoordY)
                {
                    if (tank.Intersects(new Rectangle(item.tank.bullet.CoordX, item.tank.bullet.CoordY, item.tank.bullet.Width, item.tank.bullet.Height)))
                    {
                        currentTank.tank.HP -= currentTank.tank.Damage;
                        item.tank.bullet.CoordY = -10;
                        item.tank.bullet.CoordX = -10;
                        return true;
                    }
                }
            }
            if(currentTank.tank.HP <= 0)
            {
                currentTank.tank.CoordX = 0;
                currentTank.tank.CoordY = 0;
            }

            return false;
        }
    }
}
