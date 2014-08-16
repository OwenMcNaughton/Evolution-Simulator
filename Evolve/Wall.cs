using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Wall : Entity
    {

        public Wall(double x, double y, Texture2D tex)
            : base(x, y, tex)
        {
        }

    }
}
