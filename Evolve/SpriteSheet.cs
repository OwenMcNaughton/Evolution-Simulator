using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class SpriteSheet
    {
        private Texture2D sheet;

        //Number of sprite images across and down
        public int columns { get; private set; }
        public int rows { get; private set; }

        //Dimensions of the every sprite sub image
        public int spriteWidth { get; private set; }
        public int spriteHeight { get; private set; }

        private Texture2D[,] sprites;

        public SpriteSheet(Texture2D s, int c, int r, int w, int h)
        {
            this.sheet = s;

            this.columns = c;
            this.rows = r;

            this.spriteWidth = w;
            this.spriteHeight = h;

            initSprites();
        }

        private void initSprites()
        {
            this.sprites = new Texture2D[this.columns, this.rows];

            //Converting original image to 1D color array...
            Color[] imageData = new Color[this.sheet.Width*this.sheet.Height];
            sheet.GetData<Color>(imageData);

            //... and then to a 2D color array for ease of access
            Color[,] imageData2D = new Color[this.sheet.Width, this.sheet.Height];
            for (int i = 0; i != this.sheet.Width; i++)
            {
                for (int j = 0; j != this.sheet.Height; j++)
                {
                    imageData2D[i, j] = imageData[i + j * this.sheet.Width];
                }
            }

            //Initializing each element of sprites array
            for (int i = 0; i != this.columns; i++)
            {
                for (int j = 0; j != this.rows; j++)
                {
                    Color[] subImage = new Color[this.spriteWidth*this.spriteHeight];
                    
                    for(int k = 0; k != this.spriteWidth; k++) {
                         for(int l = 0; l != this.spriteHeight; l++) {
                            subImage[k + l * this.spriteWidth] = imageData2D[k + i * spriteWidth, l + j * spriteHeight];
                         }
                    }
                    Texture2D subTexture = new Texture2D(Game1.graphics.GraphicsDevice, this.spriteWidth, this.spriteHeight);
                    subTexture.SetData<Color>(subImage);

                    this.sprites[i, j] = subTexture;
                }
            }
        }

        public void drawInUse(SpriteBatch spriteBatch, int sx, int sy, int x, int y)
        {
            spriteBatch.Draw(sprites[sx, sy], new Vector2(x, y), Color.White);
        }

        public Texture2D getSprite(int x, int y)
        {
            return sprites[x, y];
        }

        public Texture2D getSprite(Point p)
        {
            return sprites[p.X, p.Y];
        }
    }
}
