﻿using System;
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
        public const int reproduce3 = 1350;
        public const int reproduce4 = 1500;

        public Food(double x, double y, int e, Texture2D tex)
            : base(x, y, tex)
        {
            Random gen = new Random();

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
