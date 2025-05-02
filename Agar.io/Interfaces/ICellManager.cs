using Agario.Cells;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface ICellManager<T> where T : Cells.Cell
    {
        public List<T> Cells { get; }
        public List<T> FreeCells { get; }
        public void AddCell(T cell);
        public void RemoveCell(T cell);
        public T GetSplitCell(T cell, float newMass, float x, float y, float currentTime);

    }
}
