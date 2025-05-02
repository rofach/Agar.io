using Agario.Cells;
using Agario.Interfaces;
using SFML.System;

namespace Agario
{
    public class Logic
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
            return CanMerge(thisCell, otherCell) && thisCell > otherCell;
        }
        public static bool CanMerge(Cell thisCell, Cell otherCell)
        {
            return GetDistanceBetweenCells(thisCell, otherCell) < thisCell.Radius;
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

                    if (cell1.IsMergeable && cell2.IsMergeable || (cell1.Acceleration || cell2.Acceleration))
                        continue;

                    Vector2f pos1 = ((Cell)cell1).Position;
                    Vector2f pos2 = ((Cell)cell2).Position;
                    Vector2f delta = pos2 - pos1;
                    float actualDistance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    float minDistance = cells[i].Radius + cells[j].Radius + 8;

                    if (actualDistance == 0)
                        continue;

                    if (actualDistance < minDistance)
                    {
                        float overlap = (minDistance - actualDistance) / 2;
                        Vector2f normal = new Vector2f(delta.X / actualDistance, delta.Y / actualDistance);
                        correctionVectors[i] -= normal * overlap;
                        correctionVectors[j] += normal * overlap;
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is Cell cell)
                {
                    cell.Position += correctionVectors[i] * 0.5f;

                    cell.Position = new Vector2f(
                        Math.Clamp(cell.Position.X, -Game.sizeX, Game.sizeX),
                        Math.Clamp(cell.Position.Y, -Game.sizeY, Game.sizeY)
                    );
                }
            }
        }

        public static void Divide(ICellManager<Cell> cellManager, List<Cell> cells, int maxDivideCount, ref float lastDivideTime, float minMass)
        {
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
                cells.Add(child);
                currentDivideCount++;
            }
        }

        public static void Merge(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is not IMergeable cell1 || !cell1.IsMergeable)
                    continue;

                for (int j = i + 1; j < cells.Count; j++)
                {
                    if(cells[j] is not IMergeable cell2 || !cell2.IsMergeable)
                        continue;
                    if (i >= cells.Count || j >= cells.Count) break;
                    if (CanMerge(cells[i], cells[j]) && cells[i] != cells[j])
                    {
                        cells[i].Mass += cells[j].Mass;
                        cells.Remove(cells[j]);
                    }
                }
            }
        }
    }
}
