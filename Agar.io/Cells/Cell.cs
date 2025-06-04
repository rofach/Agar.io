using Agario.GameLogic;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using System.Runtime.CompilerServices;

namespace Agario.Cells
{
    [JsonObject(MemberSerialization.OptIn, IsReference = true)]
    abstract public class Cell : IDrawable, IComparable
    {
        [JsonProperty]
        private float _x, _y, _mass, _radius;
        [JsonProperty]
        private Vector2f _position;
        private CircleShape _circle;
        [JsonProperty]
        private float _outLineThickness;
        public float X
        {
            get => _x;
            private set
            {
                _x = value;
                _position = new Vector2f(_x, _y);
            }
        }
        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                _position = new Vector2f(_x, _y);
            }
        }
        
        public Vector2f Position
        {
            get => new Vector2f(_x, _y);
            set
            {
                _position = value; _x = value.X; _y = value.Y;
                CirclePosition = new Vector2f(value.X, value.Y);
            }
        }
        public Vector2f CirclePosition
        {
            get => new Vector2f(_circle.Position.X, _circle.Position.Y);
            private set
            {
                _circle.Position = value;
                _circle.Origin = new Vector2f(_circle.Radius, _circle.Radius);
            }
        }
        public float Radius
        {
            get => _radius + OutLineThickness;
            private set
            {
                if (value <= 0) throw new ArgumentException("Radius must be positive");
                _radius = value;
                _circle.Radius = value;
            }
        }
        [JsonProperty(PropertyName = "CircleShape")]
        public CircleShape Circle
        {
            get => _circle;
            protected set { _circle = value; }
        }
        public float OutLineThickness
        {
            get => _outLineThickness;
            set
            {
                _outLineThickness = value;
                Circle.OutlineThickness = value;
            }
        }
        [JsonProperty(PropertyName = "Mass")]
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                Radius = CalculateRadius(_mass);
                CirclePosition = new Vector2f(X, Y);
            }
        }
        public Cell()
        {
            _circle = new CircleShape();
            _circle.FillColor = Color.White;
            OutLineThickness = 1.0f;
            _circle.OutlineColor = Color.Black;
            _circle.SetPointCount(100);

        }
        public void InitializeCircle(float x, float y, float mass)
        {
            Mass = mass;
            Position = new Vector2f(x, y);
        }
        public virtual void Draw(RenderWindow window)
        {
            if (_circle != null)
                window.Draw(_circle);
        }

        private float CalculateRadius(float mass)
        {
            return MathF.Sqrt(mass);
        }

        public int CompareTo(object? obj)
        {
            if(obj == null) return 0;
            if(obj == this) return 0;
            var cell = obj as Cell;
            if (this > cell!) return 1;
            else if (this < cell!) return -1;
            else return 0;
        }
        public bool IsBiggerThan(Cell? other)
        {
            if(other == null) return false;
            return this.Mass > other.Mass * 1.2;
        }
        public static bool operator <(Cell that, Cell other)
        {
            return (that.Mass < other.Mass);
        }
        public static bool operator >(Cell that, Cell other)
        {
            return (that.Mass > other.Mass);
        }
    }
}
