using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Food : Entity
    {
        public int energy;

        public const int reproduce1 = 1000;
        public const int reproduce2 = 1200;

        public Food(double x, double y, int e, Texture2D tex)
            : base(x, y, tex)
        {
            Random gen = new Random();

            this.energy = e;
        }

        public new Boolean Update(GameTime gameTime)
        {
            this.energy++;
            if (this.energy == reproduce1-1 || this.energy == reproduce2-1)
            {
                return true;
            }

            if (this.energy > reproduce2)
            {
                this.energy = reproduce2;
            }

            return false;
        }



    }
}
