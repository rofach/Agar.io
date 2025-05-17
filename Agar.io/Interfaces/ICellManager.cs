using Agario.Cells;
using NetTopologySuite.IO;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface ICellManager<T> where T : Cells.Cell
    {
        public int ID { get; set; }
       /* public int MaxDivideCount { get; set; }
        public float LastDivideTime { get; set; }
        public float MinMass { get; set; }*/
        public List<T> Cells { get; }
        public List<T> FreeCells { get; }
        public void AddCell(T cell);
        public void RemoveCell(T cell);
        public T GetSplitCell(T cell, float newMass, float x, float y, float currentTime);

    }
}
