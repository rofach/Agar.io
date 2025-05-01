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
        private Vector2f _startAccelerationPoint;
        private float _accelerationDistance;
        private float _speed;
        private bool _acceleration = false;
        private int _accelerationTime;
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

        public int ID { get; set; }

        public PlayerCell(float x = 0, float y = 0, float mass = 200, int id = 1) 
        {
            //X = x; Y = y;
            Position = new Vector2f(x, y);
            Mass = mass;
            CirclePosition = new Vector2f(x + Radius, y + Radius);
            Circle.FillColor = Color.White;
            Circle.Position = CirclePosition;
            Circle.OutlineColor = new Color(100, 0, 0);
            Circle.OutlineThickness = 4;
            Circle.Texture = _texture;
            Circle.SetPointCount(100);
            _speed = 2.0f;
            _accelerationTime = 1;
            DivisionTime = 0;
        }
        public Cell Split(float newMass, float x, float y, float currentTime)
        {
            var child = new PlayerCell(x, y, newMass, 1)
            {
                Acceleration = true,
                DivisionTime = currentTime,
                AccelerationDirection = Direction * 10000,
                //ПРИДУМАТИ ЯК кОНТРОЛЮВАТИ  ВІДСТАНЬ ВІД ТОЧКИ ОСКІЛЬКИ ПРИ ВЕЛИКИХ ВІДСТАНЯХ БУДЕ ВЕЛИКИЙ ПРИЖОК
                AccelerationDistance = Radius * 6,
                StartAccelerationPoint = new Vector2f(X, Y)
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
        private bool ReachedDistance()
        {
            return _accelerationDistance <= Logic.GetDistanceBetweenPoints(new Vector2f(X, Y), _startAccelerationPoint);
        }
        public void Move(RenderWindow window)
        {
            UpdateSpeed();
            var mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            if (_acceleration)
            {
                mousePos = _accelerationDirection;
            }
            float dX = mousePos.X - X;
            float dY = mousePos.Y - Y;
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            float distanceSpeed = 1;
            float elapsedTime = Timer.GameTime - DivisionTime;
            _acceleration = _acceleration && (!ReachedDistance() && elapsedTime < 2);
            float accelerateSpeed = 1.0f;
            if (_acceleration)
            {
                _speed = 1f;
                float traveledPath = Logic.GetDistanceBetweenPoints(new Vector2f(X, Y), _startAccelerationPoint);
                if(traveledPath/_accelerationDistance < 0.5f)
                {
                    accelerateSpeed = 4.0f + (traveledPath / _accelerationDistance) * (4.0f - 1.0f);
                }
                else
                {
                    accelerateSpeed = 4.0f - ((traveledPath - _accelerationDistance / 2) / (_accelerationDistance / 2)) * (4.0f - 1.0f);
                }
                if(elapsedTime > 5)
                {
                    _acceleration = false;
                    accelerateSpeed = 1.0f;
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
