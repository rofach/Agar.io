using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    internal class Virus : Cell
    {
        public Virus() 
        {
            Random random = new Random();
            X = random.Next(-Game.sizeX, Game.sizeX);
            Y = random.Next(-Game.sizeY, Game.sizeY);
            Circle = new CircleShape();
            Mass = 500;
            Radius = GetRadius(Mass);
            
            Circle.FillColor = new Color(0, 255, 0);
            Circle.SetPointCount(100);
            Circle.Position = new Vector2f(X - Radius, Y - Radius);
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
        }
    }
}
