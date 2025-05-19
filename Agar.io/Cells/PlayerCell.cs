using Agario.GameLogic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Timer = Agario.GameLogic.Timer;

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
        private float _divisionTime;
        private float _accelerationTime;
        private Font _font;
        private Text _textToDraw;
        public Vector2f AccelerationDirection
        {
            get { return _accelerationDirection; }
            set { _accelerationDirection = value; }
        }
        public float DivisionTime { get => _divisionTime; set { _divisionTime = value; _accelerationTime = value; } }
        public float AccelerationTime {  get => _accelerationTime; set => _accelerationTime = value; }
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
        public string TextToDraw
        {
            set
            {
                _textToDraw.DisplayedString = value;
            }
        }
        public Color CellColor
        {
            get { return Circle.FillColor; }
            set { Circle.FillColor = value;
                byte r = (byte)(value.R * 0.7f);
                byte g = (byte)(value.G * 0.7f);
                byte b = (byte)(value.B * 0.7f);

                Circle.OutlineColor = new Color(r, g, b, 250);
            }
        }
        public PlayerCell(float x = 0, float y = 0, float mass = 200, int id = 1) 
        {
            ID = id;
            Position = new Vector2f(x, y);
            Mass = mass;
            var rand = Game.Random;
            OutLineThickness = 2;
            CellColor = new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
            Circle.SetPointCount(100);
            _speed = 2.0f;
            _accelerationTime = 1;
            DivisionTime = 0;
            _font  = new Font("gnyrwn971.ttf");
            _textToDraw = new Text(ID.ToString(), _font, 10);
            _textToDraw.FillColor = Color.Black;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);

            FloatRect textBounds = _textToDraw.GetLocalBounds();
            _textToDraw.CharacterSize = (uint)(Radius);
            _textToDraw.Origin = new Vector2f(textBounds.Width / 2f + textBounds.Left, textBounds.Height / 2f + textBounds.Top);
            _textToDraw.Position = Position;
            window.Draw(_textToDraw);
        }
        private void UpdateSpeed()
        {
            _speed = 200.0f / (100.0f + (float)Math.Sqrt(Mass));
        }
        private bool ReachedDistance()
        {
            return _accelerationDistance <= Logic.GetDistanceBetweenPoints(new Vector2f(X, Y), _startAccelerationPoint);
        }
        public void Move(Vector2f targetPoint)
        {
            UpdateSpeed();
            if (_acceleration)
            {
                targetPoint = _accelerationDirection;
            }
            float dX = targetPoint.X - X;
            float dY = targetPoint.Y - Y;
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            float distanceSpeed = 1;
            float elapsedTime = Timer.GameTime - AccelerationTime;
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
                Position += _direction * _speed * Timer.DeltaTime * 100 * distanceSpeed * accelerateSpeed;
            }
        }
    }
}
