using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    internal class PlayerCell : Cell, IMove
    {
        private Vector2f direction;
        private float speed;
        private Vector2f position;
        public float lastDivideTime;
        Texture texture = new Texture("textures/test2.jpg");
        short maxD = 2;
        public short currentD = 0;
        public PlayerCell()
        {
            x = 0; y = 0;
            mass = 2000;
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = Color.White;
            position = new Vector2f(x + radius, y + radius);
            circle.Position = position;
            speed = 2.0f;
            circle.OutlineColor = new Color(100, 0, 0);
            circle.OutlineThickness = 4;
            circle.Texture = texture;
            circle.SetPointCount(100);
            lastDivideTime = 0;
        }
        public Vector2f Position
        {
            get
            {
                return new Vector2f(position.X + radius, position.Y + radius);
            }
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(circle);
        }

        public void Move(RenderWindow window)
        {
            var mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            float dX = mousePos.X - this.X;
            float dY = mousePos.Y - this.Y;
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            float distanceSpeed = 1;
            if (distance < this.Size)
            {
                distanceSpeed = distance / this.Size;
            }
            if (distance > speed)
            {
                if (Math.Abs(this.X) > Game.sizeX && Math.Abs(mousePos.X) > Math.Abs(this.X))
                    dX = 0;
                if (Math.Abs(this.Y) > Game.sizeY && Math.Abs(mousePos.Y) > Math.Abs(this.Y))
                    dY = 0;
                direction = new Vector2f(dX / distance, dY / distance);

                this.Circle.Position += (direction * ((speed / (float)Math.Pow(this.Mass, 1 / 10d)) * Timer.DeltaTime * 100)) * distanceSpeed;
                position = this.Circle.Position;
                this.X = this.Circle.Position.X + this.Size;
                this.Y = this.Circle.Position.Y + this.Size;
            }
        }
    }
}
