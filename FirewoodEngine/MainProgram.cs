﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(1920, 1080, "FirewoodEngine"))
            {
                game.Run(144.0);
            }
        }
    }
}
