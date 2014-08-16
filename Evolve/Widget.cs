using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Widget : Entity
    {
        private enum Types
        {
            blank,
            simpleString,
            simpleImage,
            imageSet,
            animation,
        }
        public new int type;

        private int action;
        private String name;
        private Color stringColor;
        private SpriteFont font;

        private Color backgroundColor;
        private Color highlightColor;
        private Texture2D highlightTex;

        private Boolean highlight;
        private Boolean pressed;

        public Widget(int argx, int argy, int width, int height, Color color, int argaction) 
            : base(argx, argy) 
        {
            this.action = argaction;
            base.bounds = new Rectangle(argx, argy, width, height);
            this.type = (int)Types.blank;

            Texture2D tex = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            tex.SetData<Color>(new Color[] {color});

            this.backgroundColor = color;
            this.highlightColor = new Color((this.backgroundColor.R >= 122 ? this.backgroundColor.R - 50 : this.backgroundColor.R + 50),
                                            (this.backgroundColor.G >= 122 ? this.backgroundColor.G - 50 : this.backgroundColor.G + 50),
                                            (this.backgroundColor.B >= 122 ? this.backgroundColor.B - 50 : this.backgroundColor.B + 50));
            this.highlightTex = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.highlightTex.SetData<Color>(new Color[] { this.highlightColor });

            base.texture = tex;
        }

        public Widget(int argx, int argy, String n, SpriteFont f, Color argstringColor, Color argbackgroundColor, int argaction)
            : base(argx, argy)
        {
            this.action = argaction;
            
            this.type = (int)Types.simpleString;

            this.name = n;
            this.font = f;
            base.bounds = new Rectangle(argx, argy, (int)this.font.MeasureString(name).X+4, (int)this.font.MeasureString(name).Y);

            Texture2D tex = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            tex.SetData<Color>(new Color[] {argbackgroundColor});

            this.backgroundColor = argbackgroundColor;
            this.highlightColor = new Color((this.backgroundColor.R > 122 ? this.backgroundColor.R - 50 : this.backgroundColor.R + 50),
                                            (this.backgroundColor.G > 122 ? this.backgroundColor.G - 50 : this.backgroundColor.G + 50),
                                            (this.backgroundColor.B > 122 ? this.backgroundColor.B - 50 : this.backgroundColor.B + 50));
            this.highlightTex = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.highlightTex.SetData<Color>(new Color[] { this.highlightColor });

            this.stringColor = argstringColor;

            base.texture = tex;
        }

        public Widget(int argx, int argy, Texture2D argtex, int argaction) 
            : base(argx, argy, argtex)
        {
            this.action = argaction;
            this.type = (int)Types.simpleImage;
        }

        public Widget(int argx, int argy, SpriteSheet argsheet, Point[] argcoords, int argaction) 
            : base(argx, argy, argsheet, argcoords)
        {
            this.action = argaction;
            this.type = (int)Types.imageSet;
        }

        public Widget(int argx, int argy, Animation arganim, int argaction)
            : base(argx, argy, arganim)
        {
            this.action = argaction;
            this.type = (int)Types.animation;
        }

        public void Update(GameTime gameTime, MouseState mouseState)
        {
            switch (this.type)
            {
                case (int)Types.blank: 
                    if (base.bounds.Contains(mouseState.X, mouseState.Y))
                    {
                        this.highlight = true;
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            this.pressed = true;
                        }
                        else
                        {
                            this.pressed = false;
                        }
                    }
                    else
                    {
                        this.highlight = false;
                    }; break;
                case (int)Types.simpleString: 
                    if (base.bounds.Contains(mouseState.X, mouseState.Y))
                    {
                        this.highlight = true;
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            this.pressed = true;
                        }
                        else
                        {
                            this.pressed = false;
                        }
                    }
                    else
                    {
                        this.highlight = false;
                    }; break;
                case (int)Types.simpleImage: break;
                case (int)Types.animation: base.animation.Update(gameTime); break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (this.type)
            {
                case (int)Types.blank:
                    if (this.highlight)
                    {
                        if (this.pressed)
                        {
                            spriteBatch.Draw(this.highlightTex, new Rectangle(this.bounds.X - 2, this.bounds.Y - 2, 
                                this.bounds.Width, this.bounds.Height), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(this.highlightTex, base.bounds, Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(base.texture, base.bounds, Color.White);
                    }
                    break;
                case (int)Types.simpleString:
                    if (this.highlight)
                    {
                        if (this.pressed)
                        {
                            spriteBatch.Draw(this.highlightTex, new Rectangle(this.bounds.X - 2, this.bounds.Y - 2,
                                this.bounds.Width, this.bounds.Height), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(this.highlightTex, base.bounds, Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(base.texture, base.bounds, Color.White);
                    }
                    if (this.pressed)
                    {
                        spriteBatch.DrawString(this.font, this.name, new Vector2(base.bounds.Left, base.bounds.Top - 2), this.stringColor);
                    }
                    else
                    {
                        spriteBatch.DrawString(this.font, this.name, new Vector2(base.bounds.Left + 2, base.bounds.Top), this.stringColor); 
                    }
                    
                    break;
                case (int)Types.simpleImage: 
                    spriteBatch.Draw(base.texture, base.bounds, Color.White); break;
                case (int)Types.imageSet: 
                    spriteBatch.Draw(base.sheet.getSprite(base.spriteCoords[base.spritePointer]), base.bounds, Color.White); break;
                case (int)Types.animation: base.animation.Draw(spriteBatch, base.bounds); break;

            }
        }
    }
}
