using Agario.Interfaces;
using Agario.Strategies;
using NetTopologySuite.Geometries;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells.Bots
{
    class TeleportBot : Bot
    {
        private float lastSuperPowerUsingTime;
        public TeleportBot(int id) : base(id)
        {            
            Behavior = new SafeBehavior();
        }
        public override void SuperPower()
        {
            if (Timer.GameTime - lastSuperPowerUsingTime < 10)
                return;
            int num = Objects.Random.Next(-1, 2);
            if (num == 1) return;

            float x = Objects.Random.Next(-Game.sizeX, Game.sizeX);
            float y = Objects.Random.Next(-Game.sizeY, Game.sizeY);
            var cells = Objects.GetCellsTree();
            float searchRange = Cells.Max(c => c.Radius) * 2;
            var env = new Envelope(x - searchRange, x + searchRange, y - searchRange, y + searchRange);
            int minCellsCount = cells.Query(env).Count();
            int currentCount = minCellsCount;
            Vector2f newPos = new Vector2f(x, y);
            for (int i = 0; i < 10 && currentCount > 0; i++)
            {
                if(currentCount == 0)
                {
                    newPos = new Vector2f(x, y);
                    break;
                }
                if (currentCount < minCellsCount)
                {
                    minCellsCount = currentCount;
                    newPos = new Vector2f(x, y);
                }
                x = Objects.Random.Next(-Game.sizeX, Game.sizeX);
                y = Objects.Random.Next(-Game.sizeY, Game.sizeY);
                env = new Envelope(x - searchRange, x + searchRange, y - searchRange, y + searchRange);
                currentCount = cells.Query(env).Count();
            }
            foreach (var cell in Cells)
            {
                cell.Position = newPos;
            }

        }
    }
}
