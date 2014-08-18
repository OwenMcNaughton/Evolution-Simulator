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

        public const int reproduce1 = 2000;
        public const int reproduce2 = 2500;
        public const int reproduce3 = 3000;
        public const int reproduce4 = 4000;

        public Food(double x, double y, int e, Texture2D tex)
            : base(x, y, tex)
        {
            this.energy = e;
        }

        public Boolean Update(GameTime gameTime, KeyboardState ks)
        {
            int iterations = 1;
            if(ks.IsKeyDown(Keys.F))
            {
                iterations = 100;
            }

            for (int i = 0; i != iterations; i++)
            {

                this.energy++;
                if (this.energy == reproduce1 - 1 || this.energy == reproduce2 - 1
                    || this.energy == reproduce3 - 1 || this.energy == reproduce4 - 1)
                {
                    return true;
                }

                if (this.energy > reproduce4)
                {
                    this.energy = reproduce4;
                }
            }
            
            return false;
        }



    }
}
