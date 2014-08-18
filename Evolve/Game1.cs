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

        public Boolean paused;

        public int updates;
        public const int genTime = 600;
        public static int generations;

        public int[] totalEnergyHistory;
        public DrawableRect[] energyBars;
        public static int[] totalFoodBitsEaten;
        public DrawableRect[] foodBars;
        public List<int> bestEnergyHistory;
        public Boolean showEnergy;


        public int keyMemory;
        public int[] behaviourPointer;

        public Camera cam;

        public const int mutation = 50;

        public MouseState oldMouseState;
        public Vector2[] mouseDragPos;
        public int oldScroll;
        public KeyboardState oldKeyboardState;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static SpriteBatch spriteBatchOverlay;
        public static SpriteFont font;
        public static SpriteFont smallFont;

        public DrawableRect worldBounds;

        public static List<Bot> bots;
        public static List<Food> food;
        public static List<Wall> walls;
        public Boolean computeAngle;

        public int botFocus;
        public Boolean matchRotation;

        public static Texture2D botTex;
        public static Texture2D foodTex;
        public static Texture2D wallTex;
        public static Texture2D targetTex;
        public static SpriteSheet symbols;

        public static Texture2D[] fovs;

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
            totalEnergyHistory = new int[10000];
            energyBars = new DrawableRect[1];

            this.IsMouseVisible = true;

            botFocus = -1;

            behaviourPointer = new int[2];
            behaviourPointer[0] = 0;
            behaviourPointer[1] = 0;

            cam = new Camera();
            mouseDragPos = new Vector2[3];

            totalFoodBitsEaten = new int[10000];
            bestEnergyHistory = new List<int>();

            worldBounds = new DrawableRect(0, 0, worldWidth, worldHeight, new Color(255, 150, 150), 5);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            botTex = Content.Load<Texture2D>("botTex");
            foodTex = Content.Load<Texture2D>("foodTex");
            wallTex = Content.Load<Texture2D>("wallTex");
            targetTex = Content.Load<Texture2D>("targetTex");
            Texture2D s = Content.Load<Texture2D>("symbols");

            symbols = new SpriteSheet(s, 6, 6, 40, 40);

            bots = new List<Bot>();

            fovs = new Texture2D[6];

            fovs[0] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[0].SetData<Color>(new Color[] { Color.White });

            fovs[1] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[1].SetData<Color>(new Color[] { new Color(150, 150, 250) });

            fovs[2] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[2].SetData<Color>(new Color[] { new Color(250, 150, 150) });

            fovs[3] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[3].SetData<Color>(new Color[] { new Color(150, 250, 150) });

            fovs[4] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[4].SetData<Color>(new Color[] { new Color(250, 250, 50) });

            fovs[5] = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            fovs[5].SetData<Color>(new Color[] { new Color(50, 250, 250) });

            Random gen = new Random();
            for (int i = 0; i != 50; i++)
            {
                bots.Add(new Bot(gen.Next(worldWidth), gen.Next(worldHeight), botTex, 5d, 40d, 100d));
                for (int j = 0; j != 500; j++)
                {
                    bots[i] = new Bot(gen.Next(worldWidth), gen.Next(worldHeight), botTex, 5d, 40d, 100d, bots[i].behaviour, gen, fovs);
                }
                bots[i].angle = gen.Next(360);
            }

            food = new List<Food>();
            for (int i = 0; i != 400; i++)
            {

                food.Add(new Food(gen.Next(worldWidth), gen.Next(worldHeight), gen.Next(1000) + 1000, foodTex));
            }

            walls = new List<Wall>();

            font = Content.Load<SpriteFont>("Font");
            smallFont = Content.Load<SpriteFont>("smallFont");

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteBatchOverlay = new SpriteBatch(graphics.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            UpdateInput(gameTime, keyboardState, mouseState);

            if (!paused)
            {

                UpdateFood(gameTime, keyboardState, mouseState);
                UpdateBots(gameTime, keyboardState, mouseState);

                updates++;
                if (updates > genTime && botFocus == -1)
                {
                    updates = 0;
                    PickBest();
                }

                base.Update(gameTime);
            }

            oldMouseState = mouseState;
            oldKeyboardState = keyboardState;
        }

        public void UpdateInput(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            keyMemory++;

            if (ks.IsKeyDown(Keys.Enter) && keyMemory > 10)
            {
                keyMemory = 0;
                PickBest();
            }

            if (ks.IsKeyDown(Keys.E) && keyMemory > 10)
            {
                keyMemory = 0;
                showEnergy = !showEnergy;
            }


            if (ks.IsKeyDown(Keys.Space) && keyMemory > 10)
            {
                keyMemory = 0;
                paused = !paused;
            }

            if (!paused)
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
                    cam.zoom = (width / (float)worldWidth);
                    cam.pos = new Vector2(worldWidth / 2, worldHeight / 2);
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

                if (ks.IsKeyDown(Keys.L) && !oldKeyboardState.IsKeyDown(Keys.L))
                {
                    computeAngle = !computeAngle;
                }

                if (ks.IsKeyDown(Keys.N) && !oldKeyboardState.IsKeyDown(Keys.N))
                {
                    PickBest();
                }

                if (ks.IsKeyDown(Keys.R) && !oldKeyboardState.IsKeyDown(Keys.R))
                {
                    matchRotation = !matchRotation;
                }

                if (ms.LeftButton == ButtonState.Pressed)
                {
                    Vector2 mousePos = AdjustMouse(ms);

                    walls.Add(new Wall(mousePos.X - wallTex.Width / 2,
                                       mousePos.Y - wallTex.Height / 2, wallTex));
                    for (int i = 0; i != walls.Count - 1; i++)
                    {
                        if (walls[walls.Count - 1].bounds.Contains((int)walls[i].bounds.Center.X, (int)walls[i].bounds.Center.Y))
                        {
                            walls.Remove(walls[walls.Count - 1]);
                            break;
                        }
                    }
                }

                if (ms.RightButton == ButtonState.Pressed)
                {
                    Vector2 mousePos = AdjustMouse(ms);

                    for (int i = 0; i != walls.Count; i++)
                    {
                        if (walls[i].bounds.Contains((int)mousePos.X, (int)mousePos.Y))
                        {
                            walls.Remove(walls[i]);
                            break;
                        }
                    }
                }

                if (ms.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton != ButtonState.Pressed)
                {
                    if (botFocus == -1)
                    {
                        Vector2 newMouse = AdjustMouse(ms);

                        for (int i = 0; i != bots.Count; i++)
                        {
                            if (bots[i].fov.Contains(newMouse))
                            {
                                botFocus = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        botFocus = -1;
                        cam.rotation = 0;
                    }
                }
            }
            else
            {
                UpdatePauseInput(ks, ms);
            }

        }

        public void UpdatePauseInput(KeyboardState ks, MouseState ms)
        {
            if (ks.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up))
            {
                behaviourPointer[0]--;
                if (behaviourPointer[0] < 0)
                {
                    behaviourPointer[0] = 3;
                }
            }
            if (ks.IsKeyDown(Keys.Down) && oldKeyboardState.IsKeyUp(Keys.Down))
            {
                behaviourPointer[0]++;
                if (behaviourPointer[0] > 3)
                {
                    behaviourPointer[0] = 0;
                }
            }
            if (ks.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyUp(Keys.Right))
            {
                behaviourPointer[1]++;
                if (behaviourPointer[1] > 3)
                {
                    behaviourPointer[1] = 0;
                }
            }
            if (ks.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyUp(Keys.Left))
            {
                behaviourPointer[1]--;
                if (behaviourPointer[1] < 0)
                {
                    behaviourPointer[1] = 3;
                }
            }

            if (botFocus != -1)
            {
                bots[botFocus].behaviour.rPoint = behaviourPointer[0];
                bots[botFocus].behaviour.cPoint = behaviourPointer[1];

                if (ms.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed)
                {
                    bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].method++;
                    if (bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].method == Command.methods)
                    {
                        bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].method = 0;
                    }
                }

                if (ms.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed)
                {
                    bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].repeats++;
                    if (bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].repeats > 20)
                    {
                        bots[botFocus].behaviour.commands[behaviourPointer[0]][behaviourPointer[1]].repeats = 0;
                    }
                }
            }
        }

        public void UpdateFood(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            Random gen = new Random();

            if (gen.Next(2) == 3)
            {
                food.Add(new Food(gen.Next(worldWidth), gen.Next(worldHeight), gen.Next(401) + 400, foodTex));
            }

            for (int i = 0; i != food.Count; i++)
            {
                if (food[i].Update(gameTime, ks))
                {
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

            if (food.Count > 500)
            {
                food.Remove(food[food.Count - 1]);

            }
        }

        public void UpdateBots(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            //GetAverageBehaviour(bots);

            if (botFocus != -1)
            {
                cam.pos = bots[botFocus].pos;
                if (matchRotation)
                {
                    cam.rotation = -(float)(bots[botFocus].angle * Math.PI / 180);
                }
            }

            if (bots.Count < 5)
            {
                Random gen = new Random();
                for (int i = 0; i != 5; i++)
                {
                    Bot newBot = new Bot(bots[i].pos.X + gen.Next(20) - 10, bots[i].pos.Y + gen.Next(20) - 10,
                        botTex, 5d, 60d, 100d, bots[i].behaviour, gen, fovs);

                    newBot.angle = gen.Next(360);

                    bots.Add(newBot);
                }
            }

            for (int i = 0; i != bots.Count; i++)
            {
                if (bots[i].dead)
                {
                    

                    //bots.Remove(bots[i]);
                    //i--;
                    //if (i == bots.Count || bots.Count == 0 || i < 0)
                    //    break;
                }

                //if (bots[i].priority == (int)Bot.Priorities.mate)
                //{
                //    Random gen = new Random();
                //    for (int j = 0; j != 5; j++)
                //    {
                //        Bot newBot = new Bot(bots[i].pos.X + gen.Next(20)-10, bots[i].pos.Y + gen.Next(20)-10, 
                //            botTex, 5d, 20d, 30d, bots[i].behaviour, gen);

                //        newBot.angle = gen.Next(360);

                //        bots.Add(newBot);
                //    }

                //    bots.Remove(bots[i]);

                //    i--;
                //    if(i < 0)
                //        break;

                //}

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

            for (int i = 0; i != food.Count; i++)
            {
                food[i].Draw(spriteBatch);
            }

            for (int i = 0; i != walls.Count; i++)
            {
                walls[i].Draw(spriteBatch);
            }

            for (int i = 0; i != bots.Count; i++)
            {
                bots[i].Draw(spriteBatch);
            }

            worldBounds.Draw(spriteBatch);

            if(showEnergy)
            {
                for (int i = 0; i != energyBars.Length; i++)
                {
                    if (energyBars[i] != null)
                        energyBars[i].Draw(spriteBatch);
                }
            }

            if (foodBars != null)
            {
                for (int i = 0; i != foodBars.Length; i++)
                {
                    if (foodBars[i] != null)
                        foodBars[i].Draw(spriteBatch);
                }
            }
            
            spriteBatch.End();

            spriteBatchOverlay.Begin();

            if (botFocus != -1)
            {
                bots[botFocus].behaviour.Draw(spriteBatchOverlay, 10);
            }
 
            spriteBatchOverlay.End();

            base.Draw(gameTime);
        }

        public void PickBest()
        {
            Random gen = new Random();
            List<Bot> argBots = new List<Bot>();

            int[] bestBots = { -1, -1, -1, -1, -1 };
            Bot[] bestBotss = new Bot[5];
            int highestEnergy = 0;
            int totalEnergy = 0;

            Bot bot = null;
            if(botFocus != -1)
            {
                bot = bots[botFocus];
            }

            for (int j = 0; j != 5; j++)
            {
                for (int i = 0; i != bots.Count; i++)
                {
                    totalEnergy += bots[i].energy;

                    if (bots[i].energy > highestEnergy)
                    {
                        bestBots[j] = i;
                        highestEnergy = bots[i].energy;
                    }
                }

                if (j == 0)
                {
                    bestEnergyHistory.Add(highestEnergy);
                }

                highestEnergy = 0;
                if (bestBots[j] < bots.Count)
                {
                    if (bestBots[j] != -1)
                    {
                        bestBotss[j] = bots[bestBots[j]];
                        bots.Remove(bots[bestBots[j]]);
                    }
                    else
                    {
                        bestBotss[j] = bots[gen.Next(bots.Count)];
                    }

                }

                if (j == 0)
                {
                    totalEnergyHistory[generations] = totalEnergy;
                }
            }
            for (int i = 0; i != bestBotss.Length; i++)
            {
                argBots.Add(bestBotss[i]);
            }

            if (botFocus == -1)
            {
                bots = new List<Bot>();
                for (int i = 0; i != 5; i++)
                {
                    for (int j = 0; j != 10; j++)
                    {
                        bots.Add(new Bot(gen.Next(worldWidth), gen.Next(worldHeight), botTex, 5d, 40d, 100d, bestBotss[i].behaviour, gen, fovs));
                        bots[i * 10 + j].angle = gen.Next(360);
                    }
                }
            }
            else
            {

                bots = new List<Bot>();
                for (int i = 0; i != 50; i++)
                {
                    bots.Add(new Bot(gen.Next(worldWidth), gen.Next(worldHeight), botTex, 5d, 40d, 100d, bot.behaviour, gen, fovs));
                    bots[i].angle = gen.Next(360);
                }
            }

            food = new List<Food>();
            for (int i = 0; i != 400; i++)
            {

                food.Add(new Food(gen.Next(worldWidth), gen.Next(worldHeight), gen.Next(1000) + 1000, foodTex));
            }

            generations++;


            if (generations % 10 == 0)
            {
                Console.WriteLine("Total Energy: ");
                for (int i = 0; i != generations; i++)
                {
                    Console.Write(totalEnergyHistory[i] + ",");
                }
                Console.WriteLine();

                Console.WriteLine("Total Food: ");
                for (int i = 0; i != generations; i++)
                {
                    Console.Write(totalFoodBitsEaten[i] + ",");
                }

                Console.WriteLine();

                Console.WriteLine("Best: ");
                for (int i = 0; i != generations; i++)
                {
                    Console.Write(bestEnergyHistory[i] + ",");
                }

                Console.WriteLine();

                for (int i = 0; i != argBots.Count; i++)
                {
                    Console.Write(argBots[i].behaviour.ToString());
                }

                Console.WriteLine();
                Console.WriteLine();

            }

            energyBars = new DrawableRect[generations];
            for (int i = 0; i != generations; i++)
            {
                int size = (int)(totalEnergyHistory[i] / (double)500);
                energyBars[i] = new DrawableRect((i*2)+10, worldHeight-10-size, 2, size, Color.White, 1);
            }

            foodBars = new DrawableRect[generations];
            for (int i = 0; i != generations; i++)
            {
                int size = totalFoodBitsEaten[i];
                foodBars[i] = new DrawableRect((i * 2) + 10, -5 - size, 2, size, Color.SpringGreen, 1);
            }

        }

        public void FocusBestBot()
        {
            int highestEnergy = -1;
            for (int i = 0; i != bots.Count; i++)
            {
                if (bots[i].energy > highestEnergy)
                {
                    botFocus = i;
                    highestEnergy = bots[i].energy;
                }
            }
        }

        public Vector2 AdjustMouse(MouseState ms)
        {
            return new Vector2(((ms.X) / cam.zoom) + (cam.pos.X - (width/2) / cam.zoom), 
                               ((ms.Y) / cam.zoom) + (cam.pos.Y - (height/2) / cam.zoom));
        }
    }

}
