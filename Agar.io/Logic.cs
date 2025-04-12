using Agario.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario
{
    public static class Logic
    {
        public static float GetDistance(Cell obj1, Cell obj2)
        {
            return (float)Math.Sqrt(Math.Pow(obj1.X - obj2.X, 2) + Math.Pow(obj1.Y - obj2.Y, 2));
        }
        public static bool CanEat(Cell thisCell, Cell otherCell)
        {
            return GetDistance(thisCell, otherCell) < thisCell.Radius;
        }
    }
}
