using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Evolve
{
    public class Behaviour
    {
        public const int pollFovGap = 5;
        public int pollFov;

        public Command[][] commands;

        public int rPoint;
        public int cPoint;

        public int explore;
        public int targetLeft;
        public int targetRight;
        public int targetForward;

        public int targetTimer;
        public const int targetForget = 30;

        public Behaviour(Command[][] c)
        {
            this.rPoint = 0;
            this.cPoint = 0;
            this.pollFov = 0;

            this.commands = c;

            this.explore = 0;
            this.targetLeft = 1;
            this.targetRight = 2;
            this.targetForward = 3;
        }

        public Behaviour(Behaviour b, Random gen, int m)
        {
            Command[][] oldCommands = b.commands;

            this.rPoint = 0;
            this.cPoint = 0;
            this.pollFov = 0;

            // 1/10 chance of a new blank routine, up to a max of 5
            if (gen.Next(m) == 0 && oldCommands.Length <= 3)
            {
                this.commands = new Command[oldCommands.Length + 1][];
            }
            else
            {
                this.commands = new Command[oldCommands.Length][];
            }

            // For each routine, 1/10 chance of a new command, up to a max of 5
            for (int i = 0; i != this.commands.Length; i++)
            {
                if (i == oldCommands.Length)
                {
                    this.commands[i] = new Command[1];
                }
                else
                {
                    if (gen.Next(m) == 0 && oldCommands[i].Length <= 3)
                    {
                        this.commands[i] = new Command[oldCommands[i].Length + 1];
                    }
                    else
                    {
                        this.commands[i] = new Command[oldCommands[i].Length];
                    }
                }
            }

            for (int i = 0; i != this.commands.Length; i++)
            {
                // If it's a new command, random method with 10 repeats
                if (i == oldCommands.Length)
                {
                    commands[i][0] = new Command(gen.Next(Command.methods), 10);
                }
                else
                {
                    for (int j = 0; j != this.commands[i].Length; j++)
                    {
                        // If new command, same story
                        if (j == oldCommands[i].Length)
                        {
                            commands[i][j] = new Command(gen.Next(Command.methods), 10);
                        }
                        else // 1/10 chance of a different method with same repeats
                        {
                            
                            if (gen.Next(m) == 0)
                            {
                                this.commands[i][j] = new Command(gen.Next(Command.methods), oldCommands[i][j].repeats);

                            }
                            else // it's the same method with... 
                            {
                                if (gen.Next(m/2) == 0) // 1/5 chance of +- (0-3) repeats 
                                {
                                    int repeats = oldCommands[i][j].repeats + (gen.Next(7) - 3);
                                    if (repeats <= 0)
                                        repeats = 1;
                                    else if (repeats > 20)
                                        repeats = 20;

                                    this.commands[i][j] = new Command(oldCommands[i][j].method, repeats);
                                }
                                else
                                {
                                    this.commands[i][j] = new Command(oldCommands[i][j].method, oldCommands[i][j].repeats);
                                }
                            }
                        }
                    }
                }
            }

            // 1/10 chance of each routine type pointer being randomized to a new one
            if (gen.Next(m) == -1)
            {
                this.explore = gen.Next(this.commands.Length);
            }
            else
            {
                this.explore = b.explore;
            }

            if (gen.Next(m) == -1)
            {
                this.targetLeft = gen.Next(this.commands.Length);
            }
            else
            {
                this.targetLeft = b.targetLeft;
            }

            if (gen.Next(m) == -1)
            {
                this.targetRight = gen.Next(this.commands.Length);
            }
            else
            {
                this.targetRight = b.targetRight;
            }

            if (gen.Next(m) == -1)
            {
                this.targetForward = gen.Next(this.commands.Length);
            }
            else
            {
                this.targetForward = b.targetForward;
            }

        }

        public int Run(Bot bot)
        {

            if (rPoint < this.commands.Length && cPoint < this.commands[rPoint].Length)
            {
                if (this.commands[rPoint][cPoint].count < this.commands[rPoint][cPoint].repeats)
                {
                    return this.commands[rPoint][cPoint].Do();
                }
                else
                {
                    this.cPoint++;
                    this.commands[rPoint][cPoint - 1].count = 0;
                    if (cPoint >= this.commands[rPoint].Length)
                    {
                        cPoint = 0;
                    }
                    return -1;
                }
            }

            return 6600;
            
        }

        public Bot Think(Bot bot)
        {
            if (this.pollFov == pollFovGap)
            {
                int storeRPoint = this.rPoint;
                pollFov = 0;

                if (bot.energy < Bot.hungerLevel)
                {
                    bot.priority = (int)Bot.Priorities.feed;
                }
                else
                {
                    //bot.priority = (int)Bot.Priorities.mate;
                }

                bot = ExamineFov(bot);

                if (bot.target.X != -1)
                {
                    if (this.targetTimer > targetForget)
                    {
                        bot.target.X = -1;
                        bot.target.Y = -1;
                    }

                    double targAngle = bot.angle - Line.Angle(bot.pos, bot.target);

                    if (targAngle < -5)
                    {
                        this.rPoint = this.targetRight;
                    }
                    else if (targAngle > 5)
                    {
                        this.rPoint = this.targetLeft;
                    }
                    else
                    {
                        this.rPoint = this.targetForward;
                    }

                    

                }
                else
                {
                    this.rPoint = this.explore;
                }

                if (this.rPoint != storeRPoint)
                {
                    this.cPoint = 0;
                }


            }
            else
            {
                this.pollFov++;
            }



            return bot;
        }

        public Bot ExamineFov(Bot bot)
        {

            foreach (Bot b in Game1.bots)
            {

            }

            Boolean noFood = true;
            Boolean sawAgain = false;
            foreach (Food f in Game1.food)
            {
                if (bot.fov.Contains(f.pos))
                {
                    noFood = false;

                    if (bot.foodTarget == null && bot.priority == (int)Bot.Priorities.feed)
                    {
                        bot.target = new Vector2(f.bounds.Center.X, f.bounds.Center.Y);
                        bot.foodTarget = f;
                    }

                    if (bot.foodTarget != null)
                    {
                        if (f.GetHashCode() == bot.foodTarget.GetHashCode())
                        {
                            this.targetTimer = 0;
                            sawAgain = true;
                        }
                    }
                }
            }
            if (noFood || bot.priority != (int)Bot.Priorities.feed)
            {
                bot.foodTarget = null;
            }
            if (!sawAgain && bot.target.X != -1)
            {
                this.targetTimer++;
            }

            foreach (Wall w in Game1.walls)
            {

            }

            return bot;
        }

        public void Draw(SpriteBatch spriteBatch, int xoffset)
        {
            for (int i = 0; i != this.commands.Length; i++)
            {
                for (int j = 0; j != this.commands[i].Length; j++)
                {
                    if (this.commands[i][j] != null)
                    {
                        switch (this.commands[i][j].method)
                        {
                            case Command.ANGULAR_LEFT:
                                spriteBatch.Draw(Game1.symbols.getSprite(0, 0), new Vector2(xoffset + j * 42, 20 + i * 80), Color.White); break;
                            case Command.ANGULAR_RIGHT:
                                spriteBatch.Draw(Game1.symbols.getSprite(1, 0), new Vector2(xoffset + j * 42, 20 + i * 80), Color.White); break;
                            case Command.FORWARD:
                                spriteBatch.Draw(Game1.symbols.getSprite(2, 0), new Vector2(xoffset + j * 42, 20 + i * 80), Color.White); break;
                            case Command.BACKWARD:
                                spriteBatch.Draw(Game1.symbols.getSprite(3, 0), new Vector2(xoffset + j * 42, 20 + i * 80), Color.White); break;
                            case Command.FULL_STOP:
                                spriteBatch.Draw(Game1.symbols.getSprite(4, 0), new Vector2(xoffset + j * 42, 20 + i * 80), Color.White); break;
                        }

                        spriteBatch.DrawString(Game1.font, this.commands[i][j].repeats + "",
                                                new Vector2(xoffset + j * 42, 38 + i * 80), Color.White);
                    }

                }
            }

            spriteBatch.DrawString(Game1.smallFont, "Explore", new Vector2(xoffset, 2 + this.explore * 80), Color.White);
            spriteBatch.DrawString(Game1.smallFont, "Left", new Vector2(xoffset + 60, 2 + this.targetLeft * 80), Color.White);
            spriteBatch.DrawString(Game1.smallFont, "Right", new Vector2(xoffset + 95, 2 + this.targetRight * 80), Color.White);
            spriteBatch.DrawString(Game1.smallFont, "Forward", new Vector2(xoffset + 140, 2 + this.targetForward * 80), Color.White);

            spriteBatch.Draw(Game1.symbols.getSprite(5, 0), new Vector2(xoffset + this.cPoint * 42,
                                60 + this.rPoint * 80), Color.White);

        }

        public new String ToString()
        {
            String r = "";

            for (int i = 0; i != this.commands.Length; i++)
            {
                for (int j = 0; j != this.commands[i].Length; j++)
                {
                    
                    switch (this.commands[i][j].method)
                    {
                        case Command.FORWARD: r += "f" + this.commands[i][j].repeats; break;
                        case Command.BACKWARD: r += "b" + this.commands[i][j].repeats; break;
                        case Command.ANGULAR_LEFT: r += "l" + this.commands[i][j].repeats; break;
                        case Command.ANGULAR_RIGHT: r += "r" + this.commands[i][j].repeats; break;
                        case Command.FULL_STOP: r += "s" + this.commands[i][j].repeats; break;
                    }
                }

                r += ";";
            }

            r += "|";

            return r;
        }
    }
}
