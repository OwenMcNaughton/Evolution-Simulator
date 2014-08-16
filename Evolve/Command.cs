using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evolve
{
    public class Command
    {

        public int method;
        public const int FORWARD = 0;
        public const int BACKWARD = 1;
        public const int ANGULAR_LEFT = 2;
        public const int ANGULAR_RIGHT = 3;
        public const int FULL_STOP = 4;

        public const int methods = 5;

        public int repeats;
        public int count;

        public Command(int m, int r)
        {
            this.method = m;
            this.repeats = r;
            this.count = 0;
        }

        public int Do()
        {
            this.count++;
            return this.method;
        }


    }
}
