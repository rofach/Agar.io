using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    sealed class PlayerCell : Cell, IMove
    {
        private Vector2f _direction;
        private float _speed;
        private Vector2f _position;
        private Texture _texture = new Texture("textures/test2.jpg");
        private bool _acceleration = false;
        private int _accelerationTime = 1;
        private Vector2f _accelerationDirection;
        public Vector2f AccelerationDirection
        {
            get { return _accelerationDirection; }
            set { _accelerationDirection = value; }
        }
        public float DivisionTime { get; set; }
        public Vector2f Direction
        {
            get { return _direction; }
        }
        public bool IsMergeable
        {
            get { return Timer.GameTime - DivisionTime >= 10.0f; }
           
        }
        public Vector2f Position
        {
            get
            {
                return new Vector2f(_position.X + radius, _position.Y + radius);
            }
        }
        public bool Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }
        public PlayerCell(float x = 0, float y = 0, float mass = 200)
        {
            this.x = x; this.y = y;
            this.mass = mass;
            
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = Color.White;
            _position = new Vector2f(x + radius, y + radius);
            circle.Position = _position;
            _speed = 2.0f;
            circle.OutlineColor = new Color(100, 0, 0);
            circle.OutlineThickness = 4;
            circle.Texture = _texture;
            circle.SetPointCount(100);
            DivisionTime = 0;
        }        
        
        public override void Draw(RenderWindow window)
        {
            window.Draw(circle);
        }
        private void UpdateSpeed()
        {
            _speed = 200.0f / (100.0f + (float)Math.Sqrt(Mass));
        }
        public void Move(RenderWindow window)
        {
            UpdateSpeed();
            var mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            if (_acceleration)
            {
                mousePos = _accelerationDirection;
            }
            float dX = mousePos.X - this.X;
            float dY = mousePos.Y - this.Y;
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            float distanceSpeed = 1;
            
            if (_acceleration && Timer.GameTime - DivisionTime > _accelerationTime)
            {
                _acceleration = false;
            }
            float accelerateSpeed = 1.0f;
            if (_acceleration)
            {
                float elapsed = Timer.GameTime - DivisionTime;
                if (elapsed < 1.0f)
                {
                    if (elapsed < 0.5f)
                    {
                        accelerateSpeed = 4.0f + (elapsed / 0.5f) * (4.0f - 1.0f);
                        
                    }
                    else
                    {
                        accelerateSpeed = 4.0f - ((elapsed - 0.5f) / 0.5f) * (4.0f - 1.0f);
                    }
                    _speed = 2.0f;
                }
                else
                {
                    accelerateSpeed = 1.0f;
                    _acceleration = false;
                }
            }
            if (distance < this.Size)
            {
                distanceSpeed = distance / this.Size;
            }
           
            if (distance > _speed)
            {
                //if (Math.Abs(this.X) > Game.sizeX && Math.Abs(mousePos.X) > Math.Abs(this.X))
                //    dX = 0;
                //if (Math.Abs(this.Y) > Game.sizeY && Math.Abs(mousePos.Y) > Math.Abs(this.Y))
                //    dY = 0;
                _direction = new Vector2f(dX / distance, dY / distance);

                this.Circle.Position += (_direction * ((_speed) * Timer.DeltaTime * 100)) * distanceSpeed * accelerateSpeed;
                _position = this.Circle.Position;
                this.X = this.Circle.Position.X + this.Size;
                this.Y = this.Circle.Position.Y + this.Size;
            }
        }
    }
}
