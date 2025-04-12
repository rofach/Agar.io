using Agario.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    internal interface IMulticellular

    {
        public List<Cell> GetCells { get; set; }
    }
}
