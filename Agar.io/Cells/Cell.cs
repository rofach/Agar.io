using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    abstract public class Cell : IDrawable
    {

        private float _x, _y, _mass, _radius;
        private Vector2f _position;
        private CircleShape? _circle;
        public float X { 
            set { _x = value; }
            get { return _x; } }
        public float Y { 
            set { _y = value; }
            get { return _y; } }
        public float Radius {
            set
            {
                if (value <= 0) throw new ArgumentException("Radius must be positive");
                _radius = value;
            }
            get { return _radius; } }
        public Vector2f CirclePosition{ 
            get { return new Vector2f(_position.X + _radius, _position.Y + _radius);}
            protected set { _position = value; } }
        public CircleShape Circle { get { return _circle; } protected set { _circle = value; } }
        public float Mass { get { return _mass; } 
            set { 
                _mass = value; 
                _radius = CalculateRadius(_mass);  
                _circle.Radius = _radius; } }

        public abstract void Draw(RenderWindow window);

        protected float CalculateRadius(float mass)
        {
            return MathF.Sqrt(mass);
        }
    }
}
