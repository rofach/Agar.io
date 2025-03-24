using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Agario.Interfaces;
using System.Security.Principal;

namespace Agario.Cells
{
    class Player : Cell, IMove
    {

        private float speed;
        private Vector2f direction;
        private Vector2f position;
        public float lastDivideTime;
        Texture texture = new Texture("textures/test2.jpg");
        short maxD = 2;
        public short currentD = 0;
        
        public Vector2f Position
        {
            get
            {
                return new Vector2f(position.X + radius, position.Y + radius);
            }
        }
        public Player(bool divided = false)
        {
            x = 0; y = 0;
            mass = 2000;
            radius = GetRadius(mass);
            circle = new CircleShape(radius);
            circle.FillColor = Color.White;
            position = new Vector2f(x + radius, y + radius);
            circle.Position = position;
            speed = 1.0f;
            circle.OutlineColor = new Color(100, 0, 0);
            circle.OutlineThickness = 4;
            circle.Texture = texture;
            circle.SetPointCount(100);
            lastDivideTime = 0;
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(circle);
        }
        public void Divide()
        {
            if (currentD < maxD)
            {
                Player player = new Player(true);
                currentD++;
                player.currentD = currentD;
                this.Mass /= 2;
                player.Mass = this.Mass;        
                player.X = this.X;
                player.Y = this.Y;
                Objects.Add(player);
                lastDivideTime = Timer.GameTime;
                
            }
        }
        void Accelerate()
        {
            
        }

        public void Unite()
        {
            if(Timer.DeltaTime - lastDivideTime > 10)
            {
                Player player = new Player(true);
                player.Mass *= 2;
                Objects.Add(player);
            }
        }
        public void Move(RenderWindow window)
        {
            
            var mousePos = window.MapPixelToCoords(Mouse.GetPosition(window));
            float dX = mousePos.X - x;
            float dY = mousePos.Y - y;
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            float distanceSpeed = 1;
            if(distance < radius)
            {
                distanceSpeed = distance / radius;
            }
            if (distance > speed)
            {
                if (Math.Abs(x) > Game.sizeX && Math.Abs(mousePos.X) > Math.Abs(x))
                    dX = 0;
                if (Math.Abs(y) > Game.sizeY && Math.Abs(mousePos.Y) > Math.Abs(y))
                    dY = 0;
                direction = new Vector2f(dX / distance, dY / distance);

                circle.Position += (direction * ((speed / (float)Math.Pow(Mass, 1/10d)) * Timer.DeltaTime * 100)) * distanceSpeed;
                position = circle.Position;
                x = circle.Position.X + radius;
                y = circle.Position.Y + radius;
            }

        }
    }
}
