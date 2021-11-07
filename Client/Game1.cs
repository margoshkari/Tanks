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
        private ClientData clientData;
        private List<Tank> tanks;
        private List<Sprite> tankSprites;
        private Sprite currentTank;
        private Map[,] wallSprites = new Map[20, 12];
        private Texture2D wallTexture;
        private int[] color = new int[3];
        private SpriteFont font;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            clientData = new ClientData();
            tanks = new List<Tank>();
            tankSprites = new List<Sprite>();
            CreateMap();
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
            wallTexture = Content.Load<Texture2D>(@"Textures\wall");
            font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            string json = string.Empty;
            tankSprites.Clear();

            SendMsg();
            GetMsg();
            if (currentTank.tank.ID == 0 && tanks.Count > 0)
            {
                currentTank.tank.ID = tanks.Last().ID;
                if (GetColor())
                    currentTank.tank.Color = this.color;
                else
                    SaveColor();
            }

            Window.Title = currentTank.tank.ID.ToString();

            foreach (var item in tanks)
            {
                tankSprites.Add(new Sprite(Content.Load<Texture2D>(@"Textures\tank"), Content.Load<Texture2D>(@"Textures\bullet"), item));
            }

            TankMove();
            BulletMove();
            TankDeath();

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
            for (int i = 0; i < wallSprites.GetLength(0); i++)
            {
                for (int j = 0; j < wallSprites.GetLength(1); j++)
                {
                    if (wallSprites[i, j].IsWall)
                    {
                        _spriteBatch.Draw(wallTexture, new Rectangle(wallSprites[i, j].CoordX, wallSprites[i, j].CoordY, wallSprites[i, j].Width, wallSprites[i, j].Height), Color.White);
                    }
                }
            }
            _spriteBatch.DrawString(font, currentTank.tank.HP.ToString(), new Vector2(currentTank.tank.CoordX - 10, currentTank.tank.CoordY - 50), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private bool TankCollision(Rectangle rect)
        {
            foreach (var item in tankSprites)
            {
                if (item.tank.ID != currentTank.tank.ID)
                {
                    if (rect.Intersects(new Rectangle(item.tank.CoordX, item.tank.CoordY, item.tankTexture.Width, item.tankTexture.Height)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private bool WallCollision(Rectangle rect)
        {
            foreach (var item in wallSprites)
            {
                if (rect.Intersects(new Rectangle(item.CoordX, item.CoordY, item.Width, item.Height)) && item.IsWall)
                {
                    return true;
                }
            }
            return false;
        }
        private void TankMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (!WallCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY - currentTank.tank.Speed - currentTank.tankTexture.Height / 2,
                        currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY - currentTank.tank.Speed,
                        currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordY -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = 0f;
                        currentTank.tank.dir = Direction.Up;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (!WallCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY + currentTank.tank.Speed - currentTank.tankTexture.Height / 2,
                        currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY + currentTank.tank.Speed,
                           currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordY += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 15.7f;
                        currentTank.tank.dir = Direction.Down;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (!WallCollision(new Rectangle(currentTank.tank.CoordX - currentTank.tank.Speed - currentTank.tankTexture.Height / 2, currentTank.tank.CoordY,
                             currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX - currentTank.tank.Speed, currentTank.tank.CoordY,
                             currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordX -= currentTank.tank.Speed;
                        currentTank.tank.Rotation = -7.85f;
                        currentTank.tank.dir = Direction.Left;
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (!WallCollision(new Rectangle(currentTank.tank.CoordX + currentTank.tank.Speed - currentTank.tankTexture.Height / 2, currentTank.tank.CoordY,
                                currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                {
                    if (!TankCollision(new Rectangle(currentTank.tank.CoordX + currentTank.tank.Speed, currentTank.tank.CoordY,
                                currentTank.tankTexture.Height, currentTank.tankTexture.Width)))
                    {
                        currentTank.tank.CoordX += currentTank.tank.Speed;
                        currentTank.tank.Rotation = 7.85f;
                        currentTank.tank.dir = Direction.Right;
                    }
                }
            }
        }
        private void BulletMove()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !currentTank.tank.bullet.isActive)
            {
                currentTank.tank.bullet.CoordY = currentTank.tank.CoordY;
                currentTank.tank.bullet.CoordX = currentTank.tank.CoordX;
                currentTank.tank.bullet.Rotation = currentTank.tank.Rotation;
                currentTank.tank.bullet.dir = currentTank.tank.dir;
                currentTank.tank.bullet.isActive = true;
            }
            if (currentTank.tank.bullet.isActive)
            {
                BulletCollision();
                if (!WallCollision(new Rectangle(currentTank.tank.bullet.CoordX, currentTank.tank.bullet.CoordY, currentTank.tank.bullet.Height, currentTank.tank.bullet.Width)))
                {
                    if (currentTank.tank.bullet.dir == Direction.Up)
                    {
                        currentTank.tank.bullet.CoordY -= currentTank.tank.bullet.Speed;
                    }
                    else if (currentTank.tank.bullet.dir == Direction.Down)
                    {
                        currentTank.tank.bullet.CoordY += currentTank.tank.bullet.Speed;
                    }
                    else if (currentTank.tank.bullet.dir == Direction.Left)
                    {
                        currentTank.tank.bullet.CoordX -= currentTank.tank.bullet.Speed;
                    }
                    else if (currentTank.tank.bullet.dir == Direction.Right)
                    {
                        currentTank.tank.bullet.CoordX += currentTank.tank.bullet.Speed;
                    }

                }
                else
                    currentTank.tank.bullet.isActive = false;
            }
            else
            {
                currentTank.tank.bullet.CoordY = -100;
                currentTank.tank.bullet.CoordX = -100;
            }
        }
        private void TankDeath()
        {
            Rectangle tank = new Rectangle(currentTank.tank.CoordX, currentTank.tank.CoordY, currentTank.tankTexture.Width, currentTank.tankTexture.Height);

            foreach (var item in tankSprites)
            {
                if (item.tank.ID != currentTank.tank.ID)
                {
                    if (tank.Intersects(new Rectangle(item.tank.bullet.CoordX, item.tank.bullet.CoordY, item.tank.bullet.Width, item.tank.bullet.Height)))
                    {
                        currentTank.tank.HP -= currentTank.tank.Damage;
                    }
                }
            }
            if (currentTank.tank.HP <= 0)
            {
                currentTank.tank.CoordX = -100;
                currentTank.tank.CoordY = -100;
            }
        }
        private void BulletCollision()
        {
            Rectangle bullet = new Rectangle(currentTank.tank.bullet.CoordX, currentTank.tank.bullet.CoordY, currentTank.tank.bullet.Width, currentTank.tank.bullet.Height);
            foreach (var item in tankSprites)
            {
                if (item.tank.ID != currentTank.tank.ID && currentTank.tank.bullet.isActive)
                {
                    if (bullet.Intersects(new Rectangle(item.tank.CoordX, item.tank.CoordY, item.tankTexture.Width, item.tankTexture.Height)))
                    {
                        currentTank.tank.bullet.CoordY = -200;
                        currentTank.tank.bullet.CoordX = -200;
                        currentTank.tank.bullet.isActive = false;
                    }
                }
            }

        }
        private void CreateMap()
        {
            for (int i = 0; i < wallSprites.GetLength(0); i++)
            {
                for (int j = 0; j < wallSprites.GetLength(1); j++)
                {
                    if (i == 0 || j == 0 || j == wallSprites.GetLength(1) - 1 || i == wallSprites.GetLength(0) - 1)
                        wallSprites[i, j] = new Map(i * 40, j * 40, true);
                    else
                        wallSprites[i, j] = new Map(i * 40, j * 40, false);
                }
            }
        }

        public void SaveColor()
        {
            if (!Directory.Exists(@"C:\ProgramData\Tanks"))
                Directory.CreateDirectory(@"C:\ProgramData\Tanks");
            if (!File.Exists(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt"))
                File.WriteAllText(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt", $"{currentTank.tank.Color[0]}:{currentTank.tank.Color[1]}:{currentTank.tank.Color[2]}");
        }
        public bool GetColor()
        {
            if (File.Exists(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt"))
            {
                this.color[0] = int.Parse(File.ReadAllText(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt").Split(":")[0]);
                this.color[1] = int.Parse(File.ReadAllText(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt").Split(":")[1]);
                this.color[2] = int.Parse(File.ReadAllText(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt").Split(":")[2]);
                return true;
            }
            return false;
        }
    }
}
