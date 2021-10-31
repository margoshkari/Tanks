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
        private Texture2D bulletTexture;
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
            bulletTexture = Content.Load<Texture2D>(@"Textures\bullet");

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
                tankSprites.Add(new Sprite(Content.Load<Texture2D>(@"Textures\tank"), item));
            }

            TankMove();
            BulletMove();
           // TankDeath();

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
                _spriteBatch.Draw(bulletTexture, new Rectangle(item.tank.bullet.CoordX, item.tank.bullet.CoordY, 20, 20), null, Color.White, item.tank.Rotation, new Vector2(bulletTexture.Width / 2f, bulletTexture.Height / 2f), SpriteEffects.None, 0f);
                _spriteBatch.Draw(item.texture, new Rectangle(item.tank.CoordX, item.tank.CoordY, item.texture.Width, item.texture.Height), null, new Color(item.tank.Color[0], item.tank.Color[1], item.tank.Color[2]), item.tank.Rotation, new Vector2(item.texture.Width / 2f, item.texture.Height / 2f), SpriteEffects.None, 0f);
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
                    if (rect.Intersects(new Rectangle(item.tank.CoordX, item.tank.CoordY, item.texture.Width, item.texture.Height)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private Keys keys = Keys.W;
        private void TankMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (currentTank.tank.CoordY - (currentTank.tank.Speed + currentTank.texture.Height / 2) > 0)
                {
                    if(!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY - currentTank.tank.Speed, 
                        currentTank.texture.Height, currentTank.texture.Width)))
                    {
                        currentTank.tank.CoordY -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = 0f;
                        keys = Keys.W;
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (currentTank.tank.CoordY + (currentTank.tank.Speed + currentTank.texture.Height / 2) < _graphics.PreferredBackBufferHeight)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY + currentTank.tank.Speed,
                           currentTank.texture.Height, currentTank.texture.Width)))
                    {
                        currentTank.tank.CoordY += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 15.7f;
                        keys = Keys.S;
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (currentTank.tank.CoordX - (currentTank.tank.Speed + currentTank.texture.Height / 2) > 0)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX - currentTank.tank.Speed, currentTank.tank.CoordY,
                             currentTank.texture.Height, currentTank.texture.Width)))
                    {
                        currentTank.tank.CoordX -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = -7.85f;
                        keys = Keys.A;
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (currentTank.tank.CoordX + (currentTank.tank.Speed + currentTank.texture.Height / 2) < _graphics.PreferredBackBufferWidth)
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX + currentTank.tank.Speed, currentTank.tank.CoordY,
                                currentTank.texture.Height, currentTank.texture.Width)))
                    {
                        currentTank.tank.CoordX += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 7.85f;
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
                Task.Factory.StartNew(() =>
                {
                    lock (currentTank.tank.bullet)
                    {
                        if (keys == Keys.W)
                        {
                            while (currentTank.tank.bullet.CoordY >= -10)
                            {
                                currentTank.tank.bullet.CoordY -= currentTank.tank.bullet.Speed;
                                if (BulletCollision())
                                    break;
                                Thread.Sleep(10);
                            }
                        }
                        else if (keys == Keys.S)
                        {
                            while (currentTank.tank.bullet.CoordY <= _graphics.PreferredBackBufferHeight + 10)
                            {
                                currentTank.tank.bullet.CoordY += currentTank.tank.bullet.Speed;
                                if (BulletCollision())
                                    break;
                                Thread.Sleep(10);
                            }
                        }
                        else if (keys == Keys.A)
                        {
                            while (currentTank.tank.bullet.CoordX >= -10)
                            {
                                currentTank.tank.bullet.CoordX -= currentTank.tank.bullet.Speed;
                                if (BulletCollision())
                                    break;
                                Thread.Sleep(10);
                            }
                        }
                        else if (keys == Keys.D)
                        {
                            while (currentTank.tank.bullet.CoordX <= _graphics.PreferredBackBufferWidth + 10)
                            {
                                currentTank.tank.bullet.CoordX += currentTank.tank.bullet.Speed;
                                if (BulletCollision())
                                    break;
                                Thread.Sleep(10);
                            }
                        }
                    }
                });
            }
        }
        private bool BulletCollision()
        {
            Rectangle bullet = new Rectangle(currentTank.tank.bullet.CoordX, currentTank.tank.bullet.CoordY, bulletTexture.Width, bulletTexture.Height);

            foreach (var item in tankSprites)
            {
                if (item.tank.CoordX != currentTank.tank.CoordX && item.tank.CoordY != currentTank.tank.CoordY)
                {
                    if (bullet.Intersects(new Rectangle(item.tank.CoordX, item.tank.CoordY, item.texture.Width, item.texture.Height)))
                    {
                        //item.tank.HP -= item.tank.Damage;
                        currentTank.tank.bullet.CoordY = -10;
                        currentTank.tank.bullet.CoordX = -10;
                        return true;
                    }
                }
            }
            return false;
        }
        //private void TankDeath()
        //{
        //    if (currentTank.tank.HP <= 0)
        //    {
        //        for (int i = 0; i < tankSprites.Count; i++)
        //        {
        //            if (tankSprites[i].tank.HP == currentTank.tank.HP)
        //                tankSprites.Remove(tankSprites[i]);
        //        }
        //        clientData.socket.Disconnect(true);
        //    }
        //}
    }
}
