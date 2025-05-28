using Agario.Cells;
using Agario.Cells.Bots;
using Agario.GameLogic;
using NetTopologySuite.Geometries;
using SFML.System;


namespace Agario.Strategies
{
    public class AggressiveBehavior : IBehavior
    {
        private bool _foundTarget = false;
        private Cell? _enemy;
       
        public Vector2f FindNewTargetPoint(Bot bot)
        {
            Vector2f target = bot.TargetPoint;
            _foundTarget = false;
            var cells = bot.Cells;
            var currentTarget = bot.TargetPoint;
            if (cells == null || cells.Count == 0)
                return currentTarget;
            var allCells = Objects.GetCellsTree();
            float maxRadius = cells.Max(c => c.Radius);
            Vector2f directionCorrect = new();

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
                    if(myCell.Mass > enemy.Mass * 1.2)
                    {
                        _foundTarget = true;
                        target = enemy.Position;
                        _enemy = enemy;
                    }
                    foreach (var cell in bot.Cells)
                    {
                        if (Logic.GetDistanceBetweenCells(enemy, cell) < cell.Radius + enemy.Radius && enemy.IsBiggerThan(cell))
                        {
                            bot.UseSuperPower();
                            break;
                        }
                        if (enemy.IsBiggerThan(cell)
                            && Logic.GetDistanceBetweenCells(enemy, cell) < cell.Radius*2 + enemy.Radius)
                        {
                            var dist = Logic.GetDistanceBetweenPoints(cell.Position, target);
                            directionCorrect += Normalize(cell.Position - enemy.Position) * dist;
                        }
                    }
                }
            }
            if(_foundTarget)
            {
                float distToTarget = Logic.GetDistanceBetweenPoints(FindAveragePosition(cells), target);
                if (cells.Count == 1 && distToTarget < maxRadius * 3 && cells.First().Mass > _enemy?.Mass*2.5)
                {
                    ((PlayerCell)cells.First()).Move(target);
                    float time = bot.LastDivideTime;
                    Logic.Divide(bot, maxDivideCount: 2, ref time, 200);
                    bot.LastDivideTime = time;
                }
                target += directionCorrect / 3;
                target = new Vector2f(Math.Clamp(target.X, -Game.MapSizeX, Game.MapSizeX),
                                     Math.Clamp(target.Y, -Game.MapSizeY, Game.MapSizeY));
            }
            else
            {
                target += directionCorrect / 2;
                target = new Vector2f(Math.Clamp(target.X, -Game.MapSizeX, Game.MapSizeX), 
                                      Math.Clamp(target.Y, -Game.MapSizeY, Game.MapSizeY));
            }
            if(GetPoint(FindAveragePosition(cells), target))
            {
                _foundTarget = false;
                _enemy = null;
                target = Logic.GenereatePoint(Game.MapSizeX, Game.MapSizeY);
            }
            return target;
        }
        private Vector2f Normalize(Vector2f v)
        {
            float mag = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
            if (mag < 0.0001f) return new Vector2f(0, 0);
            return new Vector2f(v.X / mag, v.Y / mag);
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
            return Logic.GetDistanceBetweenPoints(target, avgPos) < 10;
        }

        
    }
}
