using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;


namespace Agario.Interfaces
{
    public interface IDraw
    {
        public float X { get; set; }
        public float Y { get; set; }

        public float Size { get; set; }   
        void Draw(RenderWindow window);
    }
}
