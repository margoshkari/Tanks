using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public class Gameplay
    {
        public bool isActive;
        private ClientData clientData;
        private List<Tank> tanks;
        private List<Sprite> tankSprites;
        public Sprite currentTank;
        private Map[,] wallSprites;
        private Texture2D wallTexture;
        private int[] color;
        private SpriteFont font;
        private ContentManager content;
        public Gameplay(ContentManager content)
        {
            isActive = false;
            color = new int[3];
            this.content = content;
            tanks = new List<Tank>();
            clientData = new ClientData();
            wallSprites = new Map[12, 20];
            tankSprites = new List<Sprite>();
            CreateMap();
        }
        public void LoadContent()
        {
            clientData.socket.Connect(clientData.iPEndPoint);
            currentTank = new Sprite(content.Load<Texture2D>(@"Textures\tank"), content.Load<Texture2D>(@"Textures\bullet"), new Tank());
            wallTexture = content.Load<Texture2D>(@"Textures\wall");
            font = content.Load<SpriteFont>(@"Font\font_12");
        }
        public void Update()
        {
            string json = string.Empty;
            tankSprites.Clear();

            SendMsg();
            GetMsg();
            if (currentTank.tank.ID == 0 && tanks.Count > 0)
            {
                currentTank.tank.ID = tanks.Last().ID;
                if (GetData())
                    currentTank.tank.Color = this.color;
                else if(currentTank.tank.ID > 0)
                    SaveData();
            }

            foreach (var item in tanks)
            {
                tankSprites.Add(new Sprite(content.Load<Texture2D>(@"Textures\tank"), content.Load<Texture2D>(@"Textures\bullet"), item));
            }

            TankMove();
            BulletMove();
            TankDeath();
            Exit();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
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
                        if (item.tank.HP - currentTank.tank.Damage <= 0)
                            currentTank.tank.Score++;
                    }
                }
            }

        }
        public void SaveData()
        {
            if (!Directory.Exists(@"C:\ProgramData\Tanks"))
                Directory.CreateDirectory(@"C:\ProgramData\Tanks");

            File.WriteAllText(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt", $"{currentTank.tank.Color[0]}:{currentTank.tank.Color[1]}:{currentTank.tank.Color[2]}\nScore: {currentTank.tank.Score}");
        }
        private bool GetData()
        {
            if (File.Exists(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt"))
            {
                this.color[0] = int.Parse(File.ReadAllLines(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt")[0].Split(":")[0]);
                this.color[1] = int.Parse(File.ReadAllLines(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt")[0].Split(":")[1]);
                this.color[2] = int.Parse(File.ReadAllLines(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt")[0].Split(":")[2]);

                currentTank.tank.Score = int.Parse(File.ReadAllLines(@$"C:\ProgramData\Tanks\{currentTank.tank.ID}.txt")[1].Split("Score: ")[1]);
                return true;
            }
            return false;
        }
        private void CreateMap()
        {
            int i = 0, j = 0;
            if (File.Exists(@$"C:\ProgramData\Tanks\map.txt"))
            {
                foreach (var item in File.ReadAllText(@$"C:\ProgramData\Tanks\map.txt").ToList())
                {
                    if (item == '\n')
                    {
                        i++;
                        j = 0;
                    }
                    else
                    {
                        if (item == 'X')
                            wallSprites[i, j] = new Map(j * 40, i * 40, true);
                        else
                            wallSprites[i, j] = new Map(j * 40, i * 40, false);

                        j++;
                    }
                }
            }
        }
        private void Exit()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                SaveData();
                isActive = false;
            }
        }
    }
}
