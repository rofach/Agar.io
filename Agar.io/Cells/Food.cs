using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    sealed class Food : Cell, IMovable, IDrawable
    {
        static private Random _rand = new Random();
        private Cell _target;
        public Cell Target
        {
            get { return _target; }
            set { _target = value; }
        }
        public bool IsEaten { get; set; } = false;
        public Food(int mass = 30)
        {
            Circle = new CircleShape(Radius);
            X = _rand.Next(-Game.sizeX, Game.sizeX);
            Y = _rand.Next(-Game.sizeY, Game.sizeY);
            Mass = mass;
            this.Mass = mass;
            Radius = GetRadius(mass);

            Circle.FillColor = new Color((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            Circle.SetPointCount(20);
            IsEaten = false;
            Circle.Texture = Objects.texture;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            X = _rand.Next(-sizeX, sizeX);
            Y = _rand.Next(-sizeY, sizeY);
            Vector2f position = new Vector2f(X - Radius, Y - Radius);
            Circle.Position = position;
        }
        public void Move(RenderWindow window)
        {
            if (IsEaten)
            {
                float dX = _target.X - X;
                float dY = _target.Y - Y;
                float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
                var direction = new Vector2f(dX / distance, dY / distance);
                Circle.Position += (direction * (2 * Timer.DeltaTime * 100)) * 2;
                X = Circle.Position.X + Radius;
                Y = Circle.Position.Y + Radius;
                if (distance > Radius * 2) IsEaten = false;
            }
            else
            {
                _rand = new Random();
                float moveX = _rand.Next(-39, 40) * Timer.DeltaTime;
                float moveY = _rand.Next(-39, 40) * Timer.DeltaTime;
                if (Math.Abs(X + moveX) >= Game.sizeX)
                    moveX = 0;
                if (Math.Abs(Y + moveY) >= Game.sizeY)
                    moveY = 0;
                X += moveX;
                Y += moveY;
                Circle.Position = new Vector2f(X - Radius, Y - Radius);
            }
            
        }
    }
}
