﻿using Agario.GameLogic;

namespace Agario
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(4000, 60, 100);
            game.Run();
        }
    }
}
