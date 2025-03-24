using Agario.Interfaces;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    abstract class Cell : IDraw
    {

        protected float x, y, mass, radius;
        protected CircleShape circle;
        
        public float X { 
            set { x = value; }
            get { return x; } }
        public float Y { 
            set { y = value; }
            get { return y; } }
        public float Size { 
            set { radius = value; }
            get { return radius; } }
        public float Mass { get { return mass; } set { mass = value; radius = GetRadius(mass);  circle.Radius = radius; } }

        public abstract void Draw(RenderWindow window);

        protected float GetRadius(float mass)
        {
            return (float)Math.Sqrt(mass);
        }
    }
}
