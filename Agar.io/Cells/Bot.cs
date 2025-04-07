using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    class Bot : Cell, IMove
    {
        bool foundTarget = false;
        Cell enemy;
        Vector2f target;
        bool getPoint = false;
        static Random rand = new Random();
        float speed = 1;
        public Bot()
        {
            mass = 1000;
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
            circle.SetPointCount(20);
            ChangePos(Game.sizeX, Game.sizeY);
            circle.Texture = Objects.texture;
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            x = rand.Next(-sizeX, sizeX);
            y = rand.Next(-sizeY, sizeY);
            Vector2f position = new Vector2f(x - radius, y - radius);
            circle.Position = position;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(circle);
        }

        private bool ReachedPoint()
        {
            return x > target.X - 5 && x < target.X + 5 && y > target.Y - 5 && y < target.Y + 5;
        }
        public void Move(RenderWindow window)
        {
            if (ReachedPoint())
            {
                getPoint = true;
            }
            if (!getPoint)
            {
                float dX = target.X - x;
                float dY = target.Y - y;
                float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
                var direction = new Vector2f(dX / distance, dY / distance);
                circle.Position += (direction * (2 * Timer.DeltaTime * 100));
                x = circle.Position.X + radius;
                y = circle.Position.Y + radius;
            }
            else
            {
                // Reached point, create a new target
                //targetPoint = GeneratePoint();
                //getPoint = false;
            }
        }
    }
}
