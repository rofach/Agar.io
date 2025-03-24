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
    class Food : Cell, IMove
    {
        static Random rand = new Random();
        static Color[] colors = {Color.Blue, Color.Yellow, Color.Green, Color.Red, new Color(244, 43, 99)};
        Vector2f target;
        //Texture texture = new Texture("textures/text1.png");
        bool flag;
        public Food(int mass = 30)
        {
            this.mass = mass;
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
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
            x = rand.Next(-sizeX, sizeX);
            y = rand.Next(-sizeY, sizeY);
            Vector2f position = new Vector2f(x - radius, y - radius);
            circle.Position = position;
        }
        public void SetTarget(Vector2f target)
        {
            this.target = target;
        }
        public void SetFlag(bool fl)
        {
            flag = fl;
        }
        public void Move(RenderWindow window)
        {
            if (flag)
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
                rand = new Random();
                float moveX = rand.Next(-39, 40) * Timer.DeltaTime;
                float moveY = rand.Next(-39, 40) * Timer.DeltaTime;
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
