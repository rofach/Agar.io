using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.GameLogic
{
    public interface IDrawable
    {
        float X { get; }
        float Y { get; }
        float Radius { get; }
        void Draw(RenderWindow window);
    }
}

