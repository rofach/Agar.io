using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface ICellManager<T> : IMulticellular where T : Cells.Cell
    {
        public void AddCell(T cell);
        public void RemoveCell(T cell);

    }
}
