using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Evolve
{
    public class Animation
    {
        private Texture2D[] frames;
        public int currentFrame;
        private int totalFrames;

        private long lastUpdate;
        private int framePeriod;

        private Boolean pingpong;
        private Boolean forward;

        public Animation(SpriteSheet sheet, int startx, int starty, int endx, int endy, int fp)
        {
            this.currentFrame = 0;
            this.totalFrames = ((endy - starty + 1) * sheet.columns) - startx - ((sheet.columns - 1) - endx);
            this.framePeriod = fp;

            this.frames = new Texture2D[this.totalFrames];

            for (int i = 0; i != frames.Length; i++)
            {
                if (startx == sheet.columns)
                {
                    startx = 0;
                    starty++;
                }

                frames[i] = sheet.getSprite(startx++, starty);
            }

            this.pingpong = false;
            this.forward = true;
        }

        public Animation(SpriteSheet sheet, int startx, int starty, int endx, int endy, int fp, Boolean pp)
        {
            this.currentFrame = 0;
            this.totalFrames = ((endy - starty + 1) * sheet.columns) - startx - ((sheet.columns - 1) - endx);
            this.framePeriod = fp;

            this.frames = new Texture2D[this.totalFrames];

            for (int i = 0; i != frames.Length; i++)
            {
                if (startx == sheet.columns)
                {
                    startx = 0;
                    starty++;
                }

                frames[i] = sheet.getSprite(startx++, starty);
            }

            this.pingpong = pp;
            this.forward = true;
        }

        public void Update(GameTime gameTime)
        {
            if (this.lastUpdate > this.framePeriod)
            {
                this.lastUpdate = 0;
                if (this.forward)
                {
                    this.currentFrame++;
                }
                else
                {
                    this.currentFrame--;
                }

                if (this.currentFrame == this.totalFrames)
                {
                    
                    if (this.pingpong)
                    {
                        this.forward = false;
                        this.currentFrame -= 2;
                    }
                    else
                    {
                        this.currentFrame = 0;
                    }
                }
                else if (this.currentFrame == 0)
                {
                    this.forward = true;
                }
            }
            else
            {
                this.lastUpdate += gameTime.ElapsedGameTime.Milliseconds;
            }

            
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(frames[currentFrame], new Vector2(x, y), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle r)
        {
            spriteBatch.Draw(frames[currentFrame], r, Color.White);
        }

        public Texture2D GetFrame(int i)
        {
            return this.frames[i];
        }




    }
}