using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Agario.Cells
{
    sealed class PlayerCell : Cell, IMergeable
    {
        private Vector2f _direction;
        private Vector2f _accelerationDirection;
        private float _speed;
        private bool _acceleration = false;
        private int _accelerationTime = 1;
        private Texture _texture = new Texture("textures/test2.jpg");
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
            get { return Timer.GameTime - DivisionTime >= 30.0f; }
           
        }
        public bool Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public int ID { get; set; }

        public PlayerCell(float x = 0, float y = 0, float mass = 200, int id = 1)
        {
            X = x; Y = y;
            Circle = new CircleShape(Radius);  
            Mass = mass;
            Circle.FillColor = Color.White;
            CirclePosition = new Vector2f(x + Radius, y + Radius);
            Circle.Position = CirclePosition;
            _speed = 2.0f;
            Circle.OutlineColor = new Color(100, 0, 0);
            Circle.OutlineThickness = 4;
            Circle.Texture = _texture;
            Circle.SetPointCount(100);
            Radius = GetRadius(Mass) + Circle.OutlineThickness;
            DivisionTime = 0;
        }
        public Cell Split(float newMass, float x, float y, float currentTime)
        {
            var child = new PlayerCell(x, y, newMass, 1)
            {
                Acceleration = true,
                DivisionTime = currentTime,
                AccelerationDirection = Direction * 10000000
            };
            return child;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
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
                }
                else
                {
                    accelerateSpeed = 1.0f;
                    _acceleration = false;
                }
            }
            if (distance < Radius)
            {
                distanceSpeed = distance / Radius;
            }
           
            if (distance > _speed)
            {
                _direction = new Vector2f(dX / distance, dY / distance);
                Circle.Position += _direction * _speed * Timer.DeltaTime * 100 * distanceSpeed * accelerateSpeed;
                CirclePosition = Circle.Position;
                X = Circle.Position.X + Radius;
                Y = Circle.Position.Y + Radius;
            }
        }
    }
}
