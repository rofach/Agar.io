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
    sealed class Food : Cell, IMove, IDraw
    {
        static private Random _rand = new Random();
        //static private Color[] color_s = {Color.Blue, Color.Yellow, Color.Green, Color.Red, new Color(244, 43, 99)};
        private Vector2f _target;
        private bool flag;
        public Food(int mass = 30)
        {
            this.mass = mass;
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = new Color((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            circle.SetPointCount(20);
            ChangePos(Game.sizeX, Game.sizeY);
            flag = false;
            circle.Texture = Objects.texture;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(circle);
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            x = _rand.Next(-sizeX, sizeX);
            y = _rand.Next(-sizeY, sizeY);
            Vector2f position = new Vector2f(x - radius, y - radius);
            circle.Position = position;
        }
        public void SetTarget(Vector2f target)
        {
            this._target = target;
        }
        public void SetMoveToTarget(bool fl)
        {
            flag = fl;
        }
        public void Move(RenderWindow window)
        {
            if (flag)
            {
                float dX = _target.X - x;
                float dY = _target.Y - y;
                float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
                var direction = new Vector2f(dX / distance, dY / distance);
                circle.Position += (direction * (2 * Timer.DeltaTime * 100));
                x = circle.Position.X + radius;
                y = circle.Position.Y + radius;
            }
            else
            {
                _rand = new Random();
                float moveX = _rand.Next(-39, 40) * Timer.DeltaTime;
                float moveY = _rand.Next(-39, 40) * Timer.DeltaTime;
                if (Math.Abs(x + moveX) >= Game.sizeX)
                    moveX = 0;
                if (Math.Abs(y + moveY) >= Game.sizeY)
                    moveY = 0;
                x += moveX;
                y += moveY;
                circle.Position = new Vector2f(x - radius, y - radius);
            }
            
        }
    }
}
