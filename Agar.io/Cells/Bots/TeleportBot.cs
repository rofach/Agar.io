using Agario.GameLogic;
using Agario.Strategies;
using NetTopologySuite.Geometries;
using SFML.Graphics;
using SFML.System;
using Timer = Agario.GameLogic.Timer;

namespace Agario.Cells.Bots
{
    class TeleportBot : Bot
    {
        private float lastSuperPowerUsingTime;
        public TeleportBot(int id) : base(id)
        {            
            Behavior = new AggressiveBehavior();
        }
        public override void UseSuperPower()
        {
            if (Timer.GameTime - lastSuperPowerUsingTime < 120)
                return;
            int num = Logic.Random.Next(-1, 2);
            if (num == 1) return;

            float x = Logic.Random.Next(-Game.MapSizeX, Game.MapSizeX);
            float y = Logic.Random.Next(-Game.MapSizeY, Game.MapSizeY);
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
                x = Logic.Random.Next(-Game.MapSizeX, Game.MapSizeX);
                y = Logic.Random.Next(-Game.MapSizeY, Game.MapSizeY);
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
