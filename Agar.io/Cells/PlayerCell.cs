using Agario.GameLogic;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using Timer = Agario.GameLogic.Timer;

namespace Agario.Cells
{
    [JsonObject(MemberSerialization.OptIn, IsReference = true)]
    sealed public class PlayerCell : Cell, IMergeable
    {
        [JsonProperty]
        private Vector2f _direction;
        [JsonProperty]
        private Vector2f _accelerationDirection;
        [JsonProperty]
        private Vector2f _startAccelerationPoint;
        [JsonProperty]
        private float _accelerationDistance;
        [JsonProperty]
        private float _speed;
        [JsonProperty]
        private bool _acceleration = false;
        [JsonProperty]
        private float _divisionTime;
        [JsonProperty]
        private float _accelerationTime;
        [JsonProperty]
        private float _timeToMerge = 0.0f;
        [JsonProperty]
        private int _id;

        private Text _textToDraw;
        private string _textToDrawString;

        public Vector2f AccelerationDirection
        {
            get { return _accelerationDirection; }
            set { _accelerationDirection = value; }
        }
        public float DivisionTime
        {
            get => _divisionTime; 
            set
            {
                _divisionTime = value; 
                _accelerationTime = value;
            }
        }
        public float AccelerationTime {  get => _accelerationTime; set => _accelerationTime = value; }
        public Vector2f Direction
        {
            get { return _direction; }
        }
        public float TimeToMerge
        {
            get { return _timeToMerge; }
            set { _timeToMerge = value; }
        }
        public bool IsMergeable
        {
            get { return Timer.GameTime - DivisionTime >= _timeToMerge; }
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
        [JsonProperty]
        public int ID { get => _id; set => _id = value; }
        [JsonProperty]
        public string TextToDraw
        {
            get =>  _textToDrawString;
            set
            {
                _textToDraw.DisplayedString = value;
                _textToDrawString = value;
            }
        }
        [JsonProperty]
        public Color CellColor
        {
            get { return Circle.FillColor; }
            set { 
                Circle.FillColor = value;
                byte r = (byte)(value.R * 0.7f);
                byte g = (byte)(value.G * 0.7f);
                byte b = (byte)(value.B * 0.7f);

                Circle.OutlineColor = new Color(r, g, b, 250);
            }
        }
        [JsonConstructor]
        public PlayerCell(float x = 0, float y = 0, float mass = 200, int id = 1) : base()
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
            _timeToMerge = 30f;
            DivisionTime = 0;

            if (string.IsNullOrEmpty(_textToDrawString)) // for deserialization
                _textToDrawString = ID.ToString();
            _textToDraw = new Text(_textToDrawString, Game.GameFont, 10);
            _textToDraw.FillColor = Color.Black;
        }

        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
            FloatRect textBounds = _textToDraw.GetLocalBounds();
            _textToDraw.Scale = new Vector2f(0.3f, 0.3f);
            _textToDraw.CharacterSize = (uint)(Radius*2);
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
