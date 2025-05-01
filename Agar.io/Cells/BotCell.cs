using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    class BotCell : Cell, IMovable
    {
        private bool _foundTarget = false;
        private Cell? _enemyToEat;
        private Vector2f _targetPoint;
        private Vector2f _direction;
        private bool _getPoint = false;
        private float _speed = 2.0f;
        static private Random _rand = new Random();
        public BotCell(float x, float y, float mass)
        {
            Circle = new CircleShape(Radius);
            this.X = x;
            this.Y = y;
            this.Mass = mass;
            Circle.FillColor = new Color((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            Circle.SetPointCount(100);
            ChangePos(Game.sizeX, Game.sizeY);
            //Circle.Texture = Objects.texture;
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            X = _rand.Next(-sizeX, sizeX);
            Y = _rand.Next(-sizeY, sizeY);
            Vector2f position = new Vector2f(X - Radius, Y - Radius);
            Circle.Position = position;
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
                Circle.Position += (_direction * (_speed * Timer.DeltaTime * 100));
                X = Circle.Position.X + Radius;
                Y = Circle.Position.Y + Radius;
            }
            else
            {
                _targetPoint = new Vector2f(_rand.Next(-Game.sizeX, Game.sizeX), _rand.Next(-Game.sizeY, Game.sizeY));
                _getPoint = false;
            }
        }
    }
}
