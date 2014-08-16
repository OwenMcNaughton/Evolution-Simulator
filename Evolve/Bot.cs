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
        public double size;

        public double fovAngle;
        public double fovDistance;
        
        public double maxVelocity;
        public double maxAngularVelocity;
        public double maxaccel;

        public Vector2 target;

        public int energy;
        public int hungerLevel;

        public int energyImpart;

        public Triangle fov;
        public Texture2D fovColor;

        private enum Priorities
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
        }

        public Bot(double x, double y, Texture2D tex, double size, double fa, double fd)
            : base(x, y, tex)
        {
            this.fov = new Triangle(new Vector2((float)x, (float)y), base.angle, fa, fd);
            this.fovAngle = fa;
            this.fovDistance = fd;

            this.fovColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.fovColor.SetData<Color>(new Color[] { Color.White });
        }

        public void AccelerateForward()
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

        public void AccelerateBackward()
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

        public void ChangleAngularVel(double mag, Boolean left)
        {
            if (left)
            {
                base.angularVelocity -= mag;
            }
            else
            {
                base.angularVelocity += mag;
            }

        }

        public void ExamineFov(List<Bot> bots, List<Food> food, List<Wall> walls)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(base.texture, base.bounds, null, Color.White);
            spriteBatch.Draw(base.texture, base.bounds, null, Color.White, (float)Triangle.ToRadian(base.angle), base.texCenter, SpriteEffects.None, 0);
            this.fov.Draw(spriteBatch, this.fovColor);
        }

        public void Update(GameTime gameTime, KeyboardState ks)
        {
            for (int i = 0; i != Game1.food.Count; i++)
            {
                if(this.bounds.Intersects(Game1.food[i].bounds))
                {
                    this.energy += 10;
                    Game1.food[i].energy -= 10;
                    if(Game1.food[i].energy <= 0)
                    {
                        Game1.food.Remove(Game1.food[i]);
                        i--;
                    }
                }
            }

            for (int i = 0; i != Game1.walls.Count; i++)
            {
                if (Game1.walls[i].bounds.Contains((int)this.center.X, (int)this.center.Y))
                {
                    //double angle = Line.Angle(new Vector2(this.bounds.Center.X, this.bounds.Center.Y),
                        //new Vector2(Game1.walls[i].bounds.Center.X, Game1.walls[i].bounds.Center.Y));
                    //int a = 1;
                }  
            }

            if(ks.IsKeyDown(Keys.Left)) 
            {
                this.ChangleAngularVel(.1, true);
            } 
            else if(ks.IsKeyDown(Keys.Right))
            {
                this.ChangleAngularVel(.1, false);
            }
            else if (ks.IsKeyDown(Keys.Up))
            {
                this.AccelerateForward();
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                this.AccelerateBackward();
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
            
            base.accel = .02;

            base.pos = Vector2.Add(base.pos, this.vel);
            base.center = Vector2.Add(base.center, this.vel);
            base.bounds = new Rectangle((int)base.pos.X, (int)base.pos.Y, base.texture.Width, base.texture.Height);
            this.fov = new Triangle(base.pos, base.angle, this.fovAngle, this.fovDistance);
        }



    }
}
