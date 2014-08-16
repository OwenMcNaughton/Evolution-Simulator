using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Agent : Entity
    {
        public Vector2 vel;

        public double accel;

        public double angle;
        public double angularVelocity;

        public Agent(double x, double y)
            : base(x, y)
        {
            this.vel = new Vector2(0, 0);
            this.accel = 0;
            this.angle = 0;
            this.angularVelocity = 0;
        }

        public Agent(double x, double y, Texture2D tex)
            : base(x, y, tex)
        {
            this.vel = new Vector2(0, 0);
            this.accel = 0;
            this.angle = 0;
            this.angularVelocity = 0;
        }

        public Agent(double x, double y, SpriteSheet argsheet, Point[] argcoords)
            : base(x, y, argsheet, argcoords)
        {
            this.vel = new Vector2(0, 0);
            this.accel = 0;
            this.angle = 0;
            this.angularVelocity = 0;
        }

        public Agent(double x, double y, Animation arganim) 
            : base(x, y, arganim)
        {
            this.vel = new Vector2(0, 0);
            this.accel = 0;
            this.angle = 0;
            this.angularVelocity = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            base.pos = Vector2.Add(base.pos, this.vel);
            this.angle += this.angularVelocity;

            base.Update(gameTime);
        }



    }
}
