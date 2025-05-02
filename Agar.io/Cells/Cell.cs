using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    abstract public class Cell : IDrawable
    {

        private float _x, _y, _mass, _radius;
        private Vector2f _position;
        private CircleShape _circle;

        public float X { 
            private set {_x = value; }
            get { return _x; } }
        public float Y { 
            set { _y = value; }
            get { return _y; } }
        public Vector2f Position
        {
            get { return new Vector2f(_x, _y); }
            set { _position = value; _x = value.X; _y = value.Y; CirclePosition = new Vector2f(value.X - Radius, value.Y - Radius); }
        }
        public float Radius {
            private set
            {
                if (value <= 0) throw new ArgumentException("Radius must be positive");
                _radius = CalculateRadius(_mass) + (_circle?.OutlineThickness ?? 0);
            }
            get { return _radius; } }
        public Vector2f CirclePosition { 
            get { return new Vector2f(_position.X - _radius, _position.Y - _radius);}
            protected set { _circle.Position = value; } }
        public CircleShape Circle { get { return _circle; } protected set { _circle = value; } }
        public float Mass { 
            get { return _mass; } 
            set { 
                _mass = value; 
                _radius = CalculateRadius(_mass);  
                _circle.Radius = _radius;
                CirclePosition = new Vector2f(X - Radius, Y - Radius);
            } }
        public void InitializeCircle(float x, float y, float mass)
        {
            X = x; Y = y;
            Mass = mass;
            CirclePosition = new Vector2f(x + Radius, y + Radius);
            Circle.Position = CirclePosition;
        }
        public Cell()
        {
            _circle = new CircleShape();
            _circle.FillColor = Color.White;
            _circle.OutlineThickness = 1;
            _circle.OutlineColor = Color.Black;
            _circle.SetPointCount(100);

        }
        public abstract void Draw(RenderWindow window);

        protected float CalculateRadius(float mass)
        {
            return MathF.Sqrt(mass);
        }
        public static bool operator <(Cell that, Cell other)
        {
            return (that.Radius * 1.2 < other.Radius);
        }
        public static bool operator >(Cell that, Cell other)
        {
            return (that.Radius > other.Radius * 1.2);
        }

    }
}
