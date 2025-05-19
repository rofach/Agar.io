using NetTopologySuite.IO;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    public interface ICellManager<T> where T : Cell
    {
        public int ID { get; set; }
        public List<T> Cells { get; }
        public List<T> FreeCells { get; }
        public void AddCell(T cell);
        public void RemoveCell(T cell);
        public T GetSplitCell(T cell, float newMass, float x, float y, float currentTime);

    }
}
