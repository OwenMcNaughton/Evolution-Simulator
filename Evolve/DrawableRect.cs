using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class DrawableRect
    {

        public Rectangle a;
        public Rectangle b;
        public Rectangle c;
        public Rectangle d;

        public Texture2D color;

        public DrawableRect(int x, int y, int w, int h, Color col, int t)
        {
            a = new Rectangle(x, y, w, t);
            b = new Rectangle(x, y + h - t, w, t);
            c = new Rectangle(x, y, t, h);
            d = new Rectangle(x + w - t, y, t, h);

            this.color = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.color.SetData<Color>(new Color[] { col });


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(color, this.a, Color.White);
            spriteBatch.Draw(color, this.b, Color.White);
            spriteBatch.Draw(color, this.c, Color.White);
            spriteBatch.Draw(color, this.d, Color.White);
        }

    }
}
