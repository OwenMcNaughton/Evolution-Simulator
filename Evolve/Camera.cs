using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Camera
    {
        public float zoom;
        public Matrix transform;
        public Vector2 pos;
        public float rotation;

        public Camera()
        {
            zoom = 1;
            rotation = 0;
            pos = new Vector2(Game1.width/2, Game1.height/2);
        }

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public Matrix GetTransformation(GraphicsDevice graphics)
        {
            transform = Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateRotationZ(rotation) *
                                         Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Game1.width * 0.5f, Game1.height * 0.5f, 0));
            return transform;
        }

    }
}
