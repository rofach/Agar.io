using Agario.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface IMulticellular
    {
        public List<Cell> Cells { get; }
        public List<Cell> FreeCells { get; }

    }
}
