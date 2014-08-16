using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Bot : Agent
    {
        public Behaviour behaviour;

        public double size;

        public double fovAngle;
        public double fovDistance;

        public const double maxVelocity = 2;
        public const double maxAngularVelocity = 10;
        public double maxaccel;

        public Vector2 target;
        public Food foodTarget;
        public Boolean botTarget;

        public int energy;
        public const int hungerLevel = 10000;
        public Boolean dead;

        public const int energyImpart = 1000;

        public Triangle fov;
        public Texture2D fovColor;

        public enum Priorities
        {
            feed,
            mate,
            flee,
        }
        public int priority;
        public Vector2 feedTarget;
        public Vector2 mateTarget;
        public Vector2 fleeTarget;
        
        public Bot(double x, double y, double size, double fa, double fd)
            : base(x, y)
        {
            this.fov = new Triangle(new Vector2((float)x, (float)y), base.angle, fa, fd);
            this.fovAngle = fa;
            this.fovDistance = fd;

            this.fovColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.fovColor.SetData<Color>(new Color[] { Color.White });

            this.accel = .1;

            this.energy = 1000;
        }

        public Bot(double x, double y, Texture2D tex, double size, double fa, double fd)
            : base(x, y, tex)
        {
            this.fov = new Triangle(new Vector2((float)x, (float)y), base.angle, fa, fd);
            this.fovAngle = fa;
            this.fovDistance = fd;

            this.fovColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.fovColor.SetData<Color>(new Color[] { Color.White });

            Random gen = new Random();
            Command[][] oldcoms = new Command[1][];
            oldcoms[0] = new Command[1];
            oldcoms[0][0] = new Command(Command.FORWARD, 60);
            this.behaviour = new Behaviour(oldcoms);

            this.accel = .1;

            this.energy = 1000;

            this.target = new Vector2(-1, -1);
        }

        public Bot(double x, double y, Texture2D tex, double size, double fa, double fd, Behaviour b, Random gen)
            : base(x, y, tex)
        {
            this.fov = new Triangle(new Vector2((float)x, (float)y), base.angle, fa, fd);
            this.fovAngle = fa;
            this.fovDistance = fd;

            this.fovColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.fovColor.SetData<Color>(new Color[] { Color.White });

            this.behaviour = new Behaviour(b, gen);

            this.accel = .1;

            this.energy = 1000;

            this.target = new Vector2(-1, -1);
        }


        public void AccelerateForward()
        {
            if (Line.Distance(new Vector2(0, 0), this.vel) <= maxVelocity)
            {
                double tangle = base.angle;

                if (tangle > 270)
                {
                    tangle -= 270;
                    base.vel.X -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                    base.vel.Y -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                }
                else if (tangle > 180)
                {
                    tangle -= 180;
                    base.vel.X -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                    base.vel.Y += (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                }
                else if (tangle > 90)
                {
                    tangle -= 90;
                    base.vel.X += (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                    base.vel.Y += (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                }
                else
                {
                    base.vel.X += (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                    base.vel.Y -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                }
            }
        }

        public void AccelerateBackward()
        {
            if (Line.Distance(new Vector2(0, 0), this.vel) <= maxVelocity)
            {
                double tangle = base.angle;

                if (tangle > 270)
                {
                    tangle -= 270;
                    base.vel.X += (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                    base.vel.Y += (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                }
                else if (tangle > 180)
                {
                    tangle -= 180;
                    base.vel.X += (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                    base.vel.Y -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                }
                else if (tangle > 90)
                {
                    tangle -= 90;
                    base.vel.X -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                    base.vel.Y -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                }
                else
                {
                    base.vel.X -= (float)(base.accel * (Math.Sin(Triangle.ToRadian(tangle))));
                    base.vel.Y += (float)(base.accel * (Math.Sin(Triangle.ToRadian(90 - tangle))));
                }
            }
        }

        public void AngularLeft()
        {
            base.angularVelocity -= .2;
        }

        public void AngularRight()
        {
            base.angularVelocity += .2;
        }

        public void FullStop()
        {
            this.angularVelocity -= this.angularVelocity / 10;

            this.vel.X -= this.vel.X / 10;

            this.vel.Y -= this.vel.Y / 10;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(base.texture, base.bounds, null, Color.White, (float)Triangle.ToRadian(base.angle), base.texCenter, SpriteEffects.None, 0);
            this.fov.Draw(spriteBatch, this.fovColor);
        }

        public void Update(GameTime gameTime, KeyboardState ks)
        {
            this.energy--;
            if (this.energy < 0)
            {
                this.dead = true;
            }


            for (int i = 0; i != Game1.food.Count; i++)
            {
                if(this.bounds.Intersects(Game1.food[i].bounds))
                {
                    this.energy += Game1.food[i].energy;
                    if (this.foodTarget != null)
                    {
                        if (Game1.food[i].GetHashCode() == this.foodTarget.GetHashCode())
                        {
                            this.foodTarget = null;
                        }
                    }
                    Game1.food.Remove(Game1.food[i]);
                    i--;
                }
            }

            for (int i = 0; i != Game1.walls.Count; i++)
            {
                if (Game1.walls[i].bounds.Contains((int)this.center.X, (int)this.center.Y))
                {
                }  
            }

            Bot bot = this.behaviour.Think(this);

            this.Copy(bot);

            int method = -1;
            do
            {
                method = this.behaviour.Run(this);
            }
            while (method == -1);

            switch (method)
            {
                case Command.FORWARD: this.AccelerateForward(); break;
                case Command.BACKWARD: this.AccelerateBackward(); break;
                case Command.ANGULAR_LEFT: this.AngularLeft(); break;
                case Command.ANGULAR_RIGHT: this.AngularRight(); break;
                case Command.FULL_STOP: this.FullStop(); break;
            }

            base.angle += base.angularVelocity;

            if (base.angle >= 360)
            {
                base.angle -= 360;
            }
            else if (base.angle < 0)
            {
                base.angle = 360 + base.angle;
            }

            if (Math.Abs(this.angularVelocity) > maxAngularVelocity)
            {
                if (this.angularVelocity > 0)
                {
                    this.angularVelocity = maxAngularVelocity;
                }

                if (this.angularVelocity < 0)
                {
                    this.angularVelocity = -maxAngularVelocity;
                }
            }

            base.pos = Vector2.Add(base.pos, this.vel);
            base.center = Vector2.Add(base.center, this.vel);
            
            base.bounds = new Rectangle((int)base.pos.X, (int)base.pos.Y, base.texture.Width, base.texture.Height);
            this.fov = new Triangle(base.pos, base.angle, this.fovAngle, this.fovDistance);
        }

        public void Copy(Bot bot)
        {
            this.feedTarget = bot.feedTarget;
            this.mateTarget = bot.mateTarget;
            this.fleeTarget = bot.fleeTarget;

            this.priority = bot.priority;

            this.target = bot.target;

        }



    }
}
