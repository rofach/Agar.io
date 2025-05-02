using Agario.Cells;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Interfaces
{
    public interface IMergeable : IMovable
    {
        int ID { get; set; }
        float Mass { get; set; }
        float X { get; }
        float Y { get; }
        float Radius { get; }
        public bool IsMergeable { get; }
        public bool Acceleration { get; }
        Vector2f AccelerationDirection { get; set; }
        Vector2f Direction { get; }
    }
}
