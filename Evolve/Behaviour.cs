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
            this.targetLeft = 0;
            this.targetRight = 0;
            this.targetForward = 0;
        }

        public Behaviour(Behaviour b, Random gen)
        {
            Command[][] oldCommands = b.commands;

            this.rPoint = 0;
            this.cPoint = 0;
            this.pollFov = 0;

            // 1/10 chance of a new blank routine, up to a max of 5
            if (gen.Next(10) == 0 && oldCommands.Length <= 5)
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
                    if (gen.Next(10) == 0 && oldCommands[i].Length <= 5)
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
                // If it's a new command, random method with 60 repeats
                if (i == oldCommands.Length)
                {
                    commands[i][0] = new Command(gen.Next(Command.methods), 60);
                }
                else
                {
                    for (int j = 0; j != this.commands[i].Length; j++)
                    {
                        // If new command, same story
                        if (j == oldCommands[i].Length)
                        {
                            commands[i][j] = new Command(gen.Next(Command.methods), 60);
                        }
                        else // 1/10 chance of a different method with same repeats
                        {
                            
                            if (gen.Next(10) == 0)
                            {
                                this.commands[i][j] = new Command(gen.Next(Command.methods), oldCommands[i][j].repeats);

                            }
                            else // it's the same method with... 
                            {
                                if (gen.Next(5) == 0) // 1/5 chance of +- (0-99) repeats 
                                {
                                    int repeats = oldCommands[i][j].repeats + (gen.Next(20) - 10);
                                    if (repeats < 0)
                                        repeats = 1;
                                    else if (repeats > 120)
                                        repeats = 120;

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
            if (gen.Next(10) == 0)
            {
                this.explore = gen.Next(this.commands.Length);
            }
            else
            {
                this.explore = b.explore;
            }

            if (gen.Next(10) == 0)
            {
                this.targetLeft = gen.Next(this.commands.Length);
            }
            else
            {
                this.targetLeft = b.targetLeft;
            }

            if (gen.Next(10) == 0)
            {
                this.targetRight = gen.Next(this.commands.Length);
            }
            else
            {
                this.targetRight = b.targetRight;
            }

            if (gen.Next(10) == 0)
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

        public Bot Think(Bot bot)
        {
            if (this.pollFov == pollFovGap)
            {
                pollFov = 0;

                if (bot.energy < Bot.hungerLevel)
                {
                    bot.priority = (int)Bot.Priorities.feed;
                }
                else
                {
                    bot.priority = (int)Bot.Priorities.mate;
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

                    this.cPoint = 0;

                }
                else
                {
                    this.rPoint = this.explore;
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
                        bot.target = new Vector2(f.pos.X, f.pos.Y);
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
    }
}
