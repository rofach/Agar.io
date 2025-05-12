using Agario.Cells;
using NetTopologySuite.Geometries;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Strategies
{
    public class AggressiveBehavior : IStrategy
    {
        private bool foundTarget = false;
        Cell? _enemy;
        public Vector2f FindNewTargetPoint(Bot bot)
        {
            Vector2f target = bot.TargetPoint;
            foundTarget = false;
            var cells = bot.Cells;
            var currentTarget = bot.TargetPoint;
            if (cells == null || cells.Count == 0)
                return currentTarget;
            var allCells = Objects.GetCellsTree();
            float maxRadius = cells.Max(c => c.Radius);
            
            foreach (PlayerCell myCell in cells.OfType<PlayerCell>())
            {
                float range = myCell.Radius * 5;
                var env = new Envelope(
                    myCell.Position.X - range, myCell.Position.X + range,
                    myCell.Position.Y - range, myCell.Position.Y + range);

                var nearby = allCells.Query(env)
                                    .OfType<PlayerCell>()
                                    .Where(e => e != myCell);
                foreach(var enemy in nearby)
                {
                    if (enemy.ID == myCell.ID) continue;
                    if(myCell > enemy)
                    {
                        foundTarget = true;
                        target = enemy.Position;
                        this._enemy = enemy;
                    }
                }
            }
            if(foundTarget)
            {
                
                if(Logic.GetDistanceBetweenPoints(FindAveragePosition(cells), target) < maxRadius * 3
                    && cells.First().Radius > _enemy?.Radius * 2)
                {
                    float time = bot.LastDivideTime;
                    Logic.Divide(bot, 2, ref time, 200);
                    bot.LastDivideTime = time;
                }
            }
            if(GetPoint(FindAveragePosition(cells), target))
            {
                foundTarget = false;
                target = new Vector2f(Objects.Random.Next(-Game.sizeX, Game.sizeX), Objects.Random.Next(-Game.sizeY, Game.sizeY));
            }
            return target;
        }
        private Vector2f FindAveragePosition(List<Cell> cells)
        {
            float sumX = 0, sumY = 0;
            foreach (var c in cells)
            {
                sumX += c.Position.X;
                sumY += c.Position.Y;
            }
            return new Vector2f(sumX / cells.Count, sumY / cells.Count);
        }

        private bool GetPoint(Vector2f avgPos, Vector2f target)
        {
            if (Logic.GetDistanceBetweenPoints(target, avgPos) < 10)
            {
                return true;
            }
            return false;
        }
    }
}
