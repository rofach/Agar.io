using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    class BotCell : Cell, IMovable, IMergeable
    {
        private bool _foundTarget = false;
        private Cell? _enemyToEat;
        private Vector2f _targetPoint;
        private Vector2f _direction;
        private bool _getPoint = false;
        private float _speed = 2.0f;
        private Vector2f _accelerationDirection;
        private Vector2f _startAccelerationPoint;
        private float _accelerationDistance;
        private bool _acceleration = false;
        private int _accelerationTime;
        static private Random _rand = new Random();
        public BotCell(float x, float y, float mass, int id)
        {
            //Circle = new CircleShape(Radius);
            this.Mass = mass * 10;
            Circle.FillColor = new Color((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            Circle.SetPointCount(100);
            ChangePos(Game.sizeX, Game.sizeY);
            ID = id;
            //Circle.Texture = Objects.texture;
        }
        public Vector2f AccelerationDirection
        {
            get { return _accelerationDirection; }
            set { _accelerationDirection = value; }
        }
        public Vector2f Direction
        {
            get { return _direction; }
        }
        public float DivisionTime { get; set; }
        public int ID { get; set; }
        public bool IsMergeable
        {
            get { return Timer.GameTime - DivisionTime >= 30.0f; }

        }
        public bool Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }
        public float AccelerationDistance
        {
            get { return _accelerationDistance; }
            set { _accelerationDistance = value; }
        }
        public Vector2f StartAccelerationPoint
        {
            get { return _startAccelerationPoint; }
            set { _startAccelerationPoint = value; }
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            Position = new Vector2f(_rand.Next(-sizeX, sizeX), _rand.Next(-sizeY, sizeY));
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
        }

        private bool ReachedPoint()
        {
            return X > _targetPoint.X - 5 && X < _targetPoint.X + 5 && Y > _targetPoint.Y - 5 && Y < _targetPoint.Y + 5;
        }
        private void UpdateSpeed()
        {
            _speed = 200.0f / (100.0f + (float)Math.Sqrt(Mass));
        }
        public void Move(RenderWindow window)
        {
            UpdateSpeed();
            if (ReachedPoint())
            {
                _getPoint = true;
            }
            if (!_getPoint)
            {
                float dX = _targetPoint.X - X;
                float dY = _targetPoint.Y - Y;
                float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
                _direction = new Vector2f(dX / distance, dY / distance);
                Position += (_direction * (_speed * Timer.DeltaTime * 100));
            }
            else
            {
                _targetPoint = new Vector2f(_rand.Next(-Game.sizeX, Game.sizeX), _rand.Next(-Game.sizeY, Game.sizeY));
                _getPoint = false;
            }
        }
    }
}
