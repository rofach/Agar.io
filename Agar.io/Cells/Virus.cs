using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    internal class Virus : Cell
    {
        public Virus() 
        {
            Random random = new Random();
            Mass = 500;
            Position = new Vector2f(random.Next(-Game.sizeX, Game.sizeX), random.Next(-Game.sizeY, Game.sizeY));
            Circle.FillColor = new Color(0, 255, 0);
            Circle.SetPointCount(100);
        }
        public override void Draw(RenderWindow window)
        {
            window.Draw(Circle);
        }
    }
}
