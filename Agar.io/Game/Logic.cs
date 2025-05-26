using Agario.Cells;
using SFML.System;

namespace Agario.GameLogic
{
    public static class Logic
    {
        public static float GetDistanceBetweenPoints(Vector2f point1, Vector2f point2)
        {
            return (float)Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }
        public static float GetDistanceBetweenCells(Cell obj1, Cell obj2)
        {
            return (float)Math.Sqrt(Math.Pow(obj1.X - obj2.X, 2) + Math.Pow(obj1.Y - obj2.Y, 2));
        }
        public static bool CanEat(Cell thisCell, Cell otherCell)
        {
            return CanMerge(thisCell, otherCell) && thisCell.IsBiggerThan(otherCell);
        }
        public static bool CanMerge(Cell thisCell, Cell otherCell)
        {
            var dist = GetDistanceBetweenCells(thisCell, otherCell);
            return dist < thisCell.Radius || dist < otherCell.Radius;
        }

        public static void HandleCollisions(List<Cell> cells)
        {

            Vector2f[] correctionVectors = new Vector2f[cells.Count];
            for (int i = 0; i < cells.Count; i++)
            {
                correctionVectors[i] = new Vector2f(0, 0);
            }

            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = i + 1; j < cells.Count; j++)
                {
                    if (!(cells[i] is IMergeable cell1 && cells[j] is IMergeable cell2))
                        continue;

                    if (cell1.IsMergeable && cell2.IsMergeable || cell1.Acceleration || cell2.Acceleration)
                        continue;

                    Vector2f pos1 = ((Cell)cell1).Position;
                    Vector2f pos2 = ((Cell)cell2).Position;
                    Vector2f delta = pos2 - pos1;                  
                    float actualDistance = GetDistanceBetweenPoints(pos1, pos2);
                    float minDistance = cells[i].Radius + cells[j].Radius;

                    if (actualDistance == 0)
                        continue;

                    if (actualDistance < minDistance)
                    {
                        float overlap = (minDistance - actualDistance) / 2;
                        Vector2f normal = delta / actualDistance;
                        correctionVectors[i] -= normal * overlap;
                        correctionVectors[j] += normal * overlap;
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is Cell cell)
                {
                    cell.Position += correctionVectors[i] * 0.7f;

                    cell.Position = new Vector2f(
                        Math.Clamp(cell.Position.X, -Game.MapSizeX, Game.MapSizeX),
                        Math.Clamp(cell.Position.Y, -Game.MapSizeY, Game.MapSizeY)
                    );
                }
            }
        }

        public static void Divide(ICellManager<Cell> cellManager, int maxDivideCount, ref float lastDivideTime, float minMass)
        {
            var cells = cellManager.Cells;
            cells.Sort((a, b) => b.Mass.CompareTo(a.Mass));
            int currentDivideCount = cells.Count;
            foreach (var cell in cells.OfType<IMergeable>().ToList())
            {
                if (currentDivideCount >= maxDivideCount)
                    break;
                if (cell.Mass < 2 * minMass)
                    continue;
                cell.Mass /= 2;
                float currentTime = Timer.GameTime;
                Cell child = cellManager.GetSplitCell((Cell)cell, cell.Mass, cell.X, cell.Y, currentTime);
                lastDivideTime = currentTime;
                cellManager.AddCell(child);
                currentDivideCount++;
            }
        }

        public static void Merge(ICellManager<Cell> cellManager)
        {
            var cells = cellManager.Cells;
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                if (cells[i] is not IMergeable cell1 || !cell1.IsMergeable)
                    continue;

                for (int j = cells.Count - 1; j > i; j--)
                {
                    if (cells[j] is not IMergeable cell2 || !cell2.IsMergeable)
                        continue;

                    if (CanMerge(cells[i], cells[j]))
                    {
                        cell1.Mass += cell2.Mass;
                        cellManager.RemoveCell(cells[j]);
                    }
                }
            }
        }
    }
}
