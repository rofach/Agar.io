using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    internal class Virus : Cell
    {
        public Virus(float x, float y, float size)
        {
            this.X = x;
            this.Y = y;
            this.Size = size;
        }
        public override void Draw(RenderWindow window)
        {
            throw new NotImplementedException();
        }
    }
}
