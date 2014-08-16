using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Evolve
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int width = 800;
        public const int height = 480;

        public const int worldWidth = 2000;
        public const int worldHeight = 1200;

        public Camera cam;

        public MouseState oldMouseState;
        public Vector2[] mouseDragPos;
        public int oldScroll;
        public KeyboardState oldKeyboardState;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;

        public DrawableRect worldBounds;

        public static List<Bot> bots;
        public static List<Food> food;
        public static List<Wall> walls;

        public static Texture2D botTex;
        public static Texture2D foodTex;
        public static Texture2D wallTex;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;  
            graphics.PreferredBackBufferHeight = height;   
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            cam = new Camera();
            mouseDragPos = new Vector2[3];

            worldBounds = new DrawableRect(0, 0, worldWidth, worldHeight, new Color(255, 150, 150), 10);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            botTex = Content.Load<Texture2D>("botTex");
            foodTex = Content.Load<Texture2D>("foodTex");
            wallTex = Content.Load<Texture2D>("wallTex");

            bots = new List<Bot>();

            Random gen = new Random();
            for (int i = 0; i != 10; i++)
            {
                bots.Add(new Bot(gen.Next(worldWidth), gen.Next(worldHeight), botTex, 5d, 20d, 20d));
                bots[i].angle = gen.Next(360);
            }

            food = new List<Food>();
            for (int i = 0; i != 20; i++)
            {

                food.Add(new Food(gen.Next(worldWidth), gen.Next(worldHeight), gen.Next(401) + 400, foodTex));
            }

            walls = new List<Wall>();

            font = Content.Load<SpriteFont>("Font");

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            UpdateInput(gameTime, keyboardState, mouseState);

            UpdateFood(gameTime, keyboardState, mouseState);
            UpdateBots(gameTime, keyboardState, mouseState);
            

            base.Update(gameTime);
        }

        public void UpdateInput(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            if (ms.ScrollWheelValue > oldScroll)
            {
                cam.zoom += cam.zoom / 10f;
            }
            else if (ms.ScrollWheelValue < oldScroll)
            {
                cam.zoom -= cam.zoom / 10f;
            }
            oldScroll = ms.ScrollWheelValue;

            if (ks.IsKeyDown(Keys.Z) && !oldKeyboardState.IsKeyDown(Keys.Z))
            {
                cam.zoom = .4f;
                cam.pos = new Vector2(1000, 600);
            }

            if (ks.IsKeyDown(Keys.A))
            {
                cam.Move(new Vector2(-10 / cam.zoom, 0));
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                cam.Move(new Vector2(10 / cam.zoom, 0));
            }
            else if (ks.IsKeyDown(Keys.W))
            {
                cam.Move(new Vector2(0, -10 / cam.zoom));
            }
            else if (ks.IsKeyDown(Keys.S))
            {
                cam.Move(new Vector2(0, 10 / cam.zoom));
            }


            if (ms.LeftButton == ButtonState.Pressed)
            {
                walls.Add(new Wall(ms.X - wallTex.Width / 2, ms.Y - wallTex.Height / 2, wallTex));
                for (int i = 0; i != walls.Count - 1; i++)
                {
                    if (walls[walls.Count - 1].bounds.Contains((int)walls[i].bounds.Center.X, (int)walls[i].bounds.Center.Y))
                    {
                        walls.Remove(walls[walls.Count - 1]);
                        break;
                    }
                }
            }
        }

        public void UpdateFood(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            for (int i = 0; i != food.Count; i++)
            {
                if (food[i].Update(gameTime))
                {
                    Random gen = new Random();
                    switch (gen.Next(4))
                    {
                        case 0:
                            food.Add(new Food(food[i].pos.X + food[i].bounds.Width,
                            food[i].pos.Y, gen.Next(401) + 400, foodTex));
                            for (int j = 0; j != food.Count - 1; j++)
                            {
                                if (food[food.Count - 1].bounds.Contains((int)food[j].center.X, (int)food[j].center.Y))
                                {
                                    food.Remove(food[food.Count - 1]);
                                    break;
                                }
                            }
                            break;
                        case 1:
                            food.Add(new Food(food[i].pos.X - food[i].bounds.Width,
                            food[i].pos.Y, gen.Next(401) + 400, foodTex));
                            for (int j = 0; j != food.Count - 1; j++)
                            {
                                if (food[food.Count - 1].bounds.Contains((int)food[j].center.X, (int)food[j].center.Y))
                                {
                                    food.Remove(food[food.Count - 1]);
                                    break;
                                }
                            }
                            break;
                        case 2:
                            food.Add(new Food(food[i].pos.X,
                            food[i].pos.Y - food[i].bounds.Height, gen.Next(401) + 400, foodTex));
                            for (int j = 0; j != food.Count - 1; j++)
                            {
                                if (food[food.Count - 1].bounds.Contains((int)food[j].center.X, (int)food[j].center.Y))
                                {
                                    food.Remove(food[food.Count - 1]);
                                    break;
                                }
                            }
                            break;
                        case 3:
                            food.Add(new Food(food[i].pos.X,
                            food[i].pos.Y + food[i].bounds.Height, gen.Next(401) + 400, foodTex));
                            for (int j = 0; j != food.Count - 1; j++)
                            {
                                if (food[food.Count - 1].bounds.Contains((int)food[j].center.X, (int)food[j].center.Y))
                                {
                                    food.Remove(food[food.Count - 1]);
                                    break;
                                }
                            }
                            break;
                    }

                }
            }
        }

        public void UpdateBots(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            for (int i = 0; i != bots.Count; i++)
            {
                bots[i].Update(gameTime, ks);

                if (bots[i].pos.X >= worldWidth)
                {
                    bots[i].pos.X = 0;
                    bots[i].bounds = new Rectangle((int)bots[i].pos.X, (int)bots[i].pos.Y, bots[i].texture.Width, bots[i].texture.Height);
                    bots[i].center.X = bots[i].bounds.Center.X; bots[i].center.Y = bots[i].bounds.Center.Y;
                }

                if (bots[i].pos.X < 0)
                {
                    bots[i].pos.X = worldWidth - 1;
                    bots[i].bounds = new Rectangle((int)bots[i].pos.X, (int)bots[i].pos.Y, bots[i].texture.Width, bots[i].texture.Height);
                    bots[i].center.X = bots[i].bounds.Center.X; bots[i].center.Y = bots[i].bounds.Center.Y;
                }


                if (bots[i].pos.Y >= worldHeight)
                {
                    bots[i].pos.Y = 0;
                    bots[i].bounds = new Rectangle((int)bots[i].pos.X, (int)bots[i].pos.Y, bots[i].texture.Width, bots[i].texture.Height);
                    bots[i].center.X = bots[i].bounds.Center.X; bots[i].center.Y = bots[i].bounds.Center.Y;
                }

                if (bots[i].pos.Y < 0)
                {
                    bots[i].pos.Y = worldHeight - 1;
                    bots[i].bounds = new Rectangle((int)bots[i].pos.X, (int)bots[i].pos.Y, bots[i].texture.Width, bots[i].texture.Height);
                    bots[i].center.X = bots[i].bounds.Center.X; bots[i].center.Y = bots[i].bounds.Center.Y;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.Default, RasterizerState.CullNone, null, cam.GetTransformation(graphics.GraphicsDevice));

            for (int i = 0; i != bots.Count; i++)
            {
                bots[i].Draw(spriteBatch);
            }

            for (int i = 0; i != food.Count; i++)
            {
                food[i].Draw(spriteBatch);
            }

            for (int i = 0; i != walls.Count; i++)
            {
                walls[i].Draw(spriteBatch);
            }

            worldBounds.Draw(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}