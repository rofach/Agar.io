using Agario.Cells;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Strategies
{
    public interface IStrategy
    {
        Vector2f FindNewTargetPoint(Bot bot);
    }
}
