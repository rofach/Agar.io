using Agario.Cells;
using Agario.Cells.Bots;
using Agario.GameLogic;
using NetTopologySuite.Geometries;
using SFML.System;


namespace Agario.Strategies
{
    public class SafeBehavior : IBehavior
    {
        private const float BaseWeight = 1f;
        private const float AvoidWeight = 2f;
        private const float DangerAngleThreshold = 45f;
        private const float BoundaryWeight = 2f;

        public Vector2f FindNewTargetPoint(Bot bot)
        {
            var cells = bot.Cells;
            var currentTarget = bot.TargetPoint;
            if (cells == null || cells.Count == 0)
                return currentTarget;

            Vector2f center = FindAveragePosition(cells);
            Vector2f direction = Normalize(currentTarget - center) * BaseWeight;
            Vector2f oldDirection = direction;
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

                foreach (var enemy in nearby)
                {
                    if (enemy.ID == myCell.ID) continue;
                    float angle = AngleBetween(myCell.Position - enemy.Position, enemy.Direction);
                    var minCell = cells.Min();
                    if (angle < DangerAngleThreshold && enemy.Mass > minCell.Mass * 1.2)
                    {
                        Vector2f away = Normalize(center - enemy.Position);
                        direction += away * AvoidWeight;
                        foreach (var cell in bot.Cells)
                        {
                            if (Logic.GetDistanceBetweenCells(enemy, cell) < cell.Radius + enemy.Radius && enemy.IsBiggerThan(cell))
                            {
                                bot.SuperPower();
                                break;
                            }
                        }
                    }

                }
            }
            if (oldDirection.Equals(direction))
            {
                if (GetPoint(center, currentTarget))
                    return center;
                return currentTarget;
            }

            if (center.X < -Game.MapSizeX + maxRadius)
                direction += new Vector2f(1, 0) * BoundaryWeight;
            else if (center.X > Game.MapSizeX - maxRadius)
                direction += new Vector2f(-1, 0) * BoundaryWeight;

            if (center.Y < -Game.MapSizeY + maxRadius)
                direction += new Vector2f(0, 1) * BoundaryWeight;
            else if (center.Y > Game.MapSizeY - maxRadius)
                direction += new Vector2f(0, -1) * BoundaryWeight;
            Vector2f result = center + Normalize(direction) * cells.Min(c => c.Radius) * 5;
            return result;
        }
        private bool GetPoint(Vector2f avgPos, Vector2f target)
        {
            if (Logic.GetDistanceBetweenPoints(target, avgPos) < 10)
            {
                return true;
            }
            return false;
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

        private Vector2f Normalize(Vector2f v)
        {
            float mag = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
            if (mag < 0.0001f) return new Vector2f(0, 0);
            return new Vector2f(v.X / mag, v.Y / mag);
        }

        private float AngleBetween(Vector2f a, Vector2f b)
        {
            float dot = a.X * b.X + a.Y * b.Y;
            float magA = MathF.Sqrt(a.X * a.X + a.Y * a.Y);
            float magB = MathF.Sqrt(b.X * b.X + b.Y * b.Y);
            float cos = dot / (magA * magB);
            cos = Math.Clamp(cos, -1f, 1f);
            return MathF.Acos(cos) * (180 / MathF.PI);
        }
    }
}
