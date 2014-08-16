using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Triangle(Vector2 arga, Vector2 argb, Vector2 argc) 
        {
            this.a = arga;
            this.b = argb;
            this.c = argc;
        }

        public Triangle(Vector2 arga, double botAngle, double fovAngle, double fovDistance)
        {
            this.a = arga;

            double lxAngle = botAngle - fovAngle;
            double rxAngle = botAngle + fovAngle;

            if (lxAngle < 0)
                lxAngle = 360 + lxAngle;

            if (rxAngle >= 360)
                rxAngle = rxAngle - 360;

            double xLength = fovDistance/Math.Sin((ToRadian(90-fovAngle/2)));

            double xAdder = 0;
            double yAdder = 0;
            if (lxAngle >= 0 && lxAngle < 90)
            {
                xAdder = xLength * Math.Sin(ToRadian(lxAngle));
                yAdder = xLength * Math.Sin(ToRadian(90 - lxAngle));
                this.b = new Vector2(this.a.X + (float)xAdder, this.a.Y - (float)yAdder);
            }
            else if (lxAngle >= 90 && lxAngle < 180)
            {
                lxAngle -= 90;
                xAdder = xLength * Math.Sin(ToRadian(90 - lxAngle));
                yAdder = xLength * Math.Sin(ToRadian(lxAngle));
                this.b = new Vector2(this.a.X + (float)xAdder, this.a.Y + (float)yAdder);
            }
            else if (lxAngle >= 180 && lxAngle < 270)
            {
                lxAngle -= 180;
                xAdder = xLength * Math.Sin(ToRadian(lxAngle));
                yAdder = xLength * Math.Sin(ToRadian(90 - lxAngle));
                this.b = new Vector2(this.a.X - (float)xAdder, this.a.Y + (float)yAdder);
            }
            else if (lxAngle >= 270 && lxAngle < 360)
            {
                lxAngle -= 270;
                xAdder = xLength * Math.Sin(ToRadian(90 - lxAngle));
                yAdder = xLength * Math.Sin(ToRadian(lxAngle));
                this.b = new Vector2(this.a.X - (float)xAdder, this.a.Y - (float)yAdder);
            }

            if (rxAngle >= 0 && rxAngle < 90)
            {
                xAdder = xLength * Math.Sin(ToRadian(rxAngle));
                yAdder = xLength * Math.Sin(ToRadian(90 - rxAngle));
                this.c = new Vector2(this.a.X + (float)xAdder, this.a.Y - (float)yAdder);
            }
            else if (rxAngle >= 90 && rxAngle < 180)
            {
                rxAngle -= 90;
                xAdder = xLength * Math.Sin(ToRadian(90 - rxAngle));
                yAdder = xLength * Math.Sin(ToRadian(rxAngle));
                this.c = new Vector2(this.a.X + (float)xAdder, this.a.Y + (float)yAdder);
            }
            else if (rxAngle >= 180 && rxAngle < 270)
            {
                rxAngle -= 180;
                xAdder = xLength * Math.Sin(ToRadian(rxAngle));
                yAdder = xLength * Math.Sin(ToRadian(90 - rxAngle));
                this.c = new Vector2(this.a.X - (float)xAdder, this.a.Y + (float)yAdder);
            }
            else if (rxAngle >= 270 && rxAngle < 360)
            {
                rxAngle -= 270;
                xAdder = xLength * Math.Sin(ToRadian(90 - rxAngle));
                yAdder = xLength * Math.Sin(ToRadian(rxAngle));
                this.c = new Vector2(this.a.X - (float)xAdder, this.a.Y - (float)yAdder);
            }


        }

        public void Draw(SpriteBatch spriteBatch, Texture2D color)
        {
            spriteBatch.Draw(color, this.a, null, Color.White,
                         (float)Math.Atan2(this.b.Y - this.a.Y, this.b.X - this.a.X),
                         new Vector2(0f, (float)color.Height / 2),
                         new Vector2(Vector2.Distance(this.a, this.b), 1f),
                         SpriteEffects.None, 0f);

            spriteBatch.Draw(color, this.a, null, Color.White,
                         (float)Math.Atan2(this.c.Y - this.a.Y, this.c.X - this.a.X),
                         new Vector2(0f, (float)color.Height / 2),
                         new Vector2(Vector2.Distance(this.a, this.c), 1f),
                         SpriteEffects.None, 0f);

            spriteBatch.Draw(color, this.b, null, Color.White,
                         (float)Math.Atan2(this.c.Y - this.b.Y, this.c.X - this.b.X),
                         new Vector2(0f, (float)color.Height / 2),
                         new Vector2(Vector2.Distance(this.b, this.c), 1f),
                         SpriteEffects.None, 0f);
        }

        public Boolean Contains(Vector2 p)
        {
            Vector2 v0 = this.c - this.a;
            Vector2 v1 = this.b - this.a;
            Vector2 v2 = p - this.a;

            double dot00 = dot(v0, v0);
            double dot01 = dot(v0, v1);
            double dot02 = dot(v0, v2);
            double dot11 = dot(v1, v1);
            double dot12 = dot(v1, v2);

            double inv = 1 / (dot00 * dot11 - dot01 * dot01);

            double u = (dot11 * dot02 - dot01 * dot12) * inv;
            double v = (dot00 * dot12 - dot01 * dot02) * inv;

            return (u >= 0) && (v >= 0) && (u + v < 1);
        }

        public Boolean Contains(int x, int y)
        {
            Vector2 p = new Vector2(x, y);

            Vector2 v0 = this.c - this.a;
            Vector2 v1 = this.b - this.a;
            Vector2 v2 = p - this.a;

            double dot00 = dot(v0, v0);
            double dot01 = dot(v0, v1);
            double dot02 = dot(v0, v2);
            double dot11 = dot(v1, v1);
            double dot12 = dot(v1, v2);

            double inv = 1 / (dot00 * dot11 - dot01 * dot01);

            double u = (dot11 * dot02 - dot01 * dot12) * inv;
            double v = (dot00 * dot12 - dot01 * dot02) * inv;

            return (u >= 0) && (v >= 0) && (u + v < 1);
        }

        public double dot(Vector2 v0, Vector2 v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y;
        }

        public static double ToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

    }
}
