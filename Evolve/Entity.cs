using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Entity
    {
        private enum Types {
            stillImage,
            sheet,
            animation
        }

        public int type;

        public Vector2 pos;

        public Vector2 center;

        public Vector2 texCenter;

        public Boolean visible;

        public Rectangle bounds;

        public Texture2D texture;

        public SpriteSheet sheet;
        public Point[] spriteCoords;
        public int spritePointer;

        public Animation animation;

        public Entity(double argx, double argy)
        {
            this.pos = new Vector2((float)argx, (float)argy);
        }

        public Entity(double argx, double argy, Texture2D argtex)
        {
            this.pos = new Vector2((float)argx, (float)argy);

            this.texture = argtex;

            this.bounds = new Rectangle((int)this.pos.X, (int)this.pos.Y, this.texture.Width, this.texture.Height);

            this.texCenter = new Vector2(this.texture.Width / 2, this.texture.Height / 2);

            this.center = new Vector2(this.bounds.Center.X, this.bounds.Center.Y);

            this.type = (int)Types.stillImage;
        }

        public Entity(double argx, double argy, SpriteSheet argsheet, Point[] argcoords)
        {
            this.pos = new Vector2((float)argx, (float)argy);

            this.sheet = argsheet;
            this.spriteCoords = argcoords;

            this.bounds = new Rectangle((int)this.pos.X, (int)this.pos.Y, this.sheet.getSprite(this.spriteCoords[0]).Width, 
                this.sheet.getSprite(this.spriteCoords[0]).Height);

            this.center = new Vector2(this.bounds.Center.X, this.bounds.Center.Y);

            this.type = (int)Types.sheet;
        }

        public Entity(double argx, double argy, Animation arganim)
        {
            this.pos = new Vector2((float)argx, (float)argy);

            this.animation = arganim;

            this.bounds = new Rectangle((int)this.pos.X, (int)this.pos.Y, this.animation.GetFrame(0).Width, this.animation.GetFrame(0).Height);

            this.center = new Vector2(this.bounds.Center.X, this.bounds.Center.Y);

            this.type = (int)Types.animation;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            switch(this.type)
            {
                case (int)Types.stillImage: 
                    spriteBatch.Draw(this.texture, this.bounds, Color.White); break;
                case (int)Types.sheet: 
                    spriteBatch.Draw(this.sheet.getSprite(this.spriteCoords[this.spritePointer]), this.bounds, Color.White); break;
                case (int)Types.animation: 
                    this.animation.Draw(spriteBatch, this.bounds); break;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            switch (this.type)
            {
                case (int)Types.stillImage: break;
                case (int)Types.sheet: break;
                case (int)Types.animation: this.animation.Update(gameTime); break;
            }

        }




    }
}
