using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(5000);
            game.Run();

        }
    }
}
