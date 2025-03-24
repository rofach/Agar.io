using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface IMove : IDraw
    {
        public void Move(RenderWindow window);
    }
}
