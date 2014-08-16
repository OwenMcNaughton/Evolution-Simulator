using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Line
    {

        public Texture2D color;
        public double angle;
        public double length;
        public int thickness;
        public Rectangle bounds;
        public Vector2 origin;

        public Line(Vector2 a, Vector2 b, Color c, int t)
        {
            this.thickness = t;
            this.angle = Line.Angle(a, b);
            this.color = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.color.SetData<Color>(new Color[] { c });
            this.length = Line.Distance(a, b);
            this.bounds = new Rectangle((int)(a.X - this.thickness / 2), (int)a.Y, (int)1, (int)this.length);
            this.origin = a;
        }

        public static double Angle(Vector2 a, Vector2 b)
        {
            double xDiff = a.X - b.X;
            double yDiff = a.Y - b.Y;
            if (a.X < b.X && a.Y >= b.Y)
            {
                return Math.Atan2(xDiff, yDiff) * 180 / Math.PI;
            }

            if (a.X < b.X && a.Y < b.Y)
            {
                return (Math.Atan2(xDiff, yDiff) * 180 / Math.PI)+90;
            }

            if (a.X >= b.X && a.Y < b.Y)
            {
                return (Math.Atan2(xDiff, yDiff) * 180 / Math.PI)+180;
            }

            else
            {
                return (Math.Atan2(xDiff, yDiff) * 180 / Math.PI)+270;
            }
            
        }

        public static double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt((a.X - b.X)*(a.X - b.X) + (a.Y - b.Y)*(a.Y - b.Y));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.color, this.bounds, null, Color.White, (float)this.angle, this.origin, SpriteEffects.None, 0);
        }


    }
}
