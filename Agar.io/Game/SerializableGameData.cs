using Agario.Cells.Bots;
using Agario.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.GameLogic
{
    public class SerializableGameData
    {
        public Player PlayerInstance { get; set; }
        public List<Bot> Bots { get; set; } = new List<Bot>();
        public List<Virus> Viruses { get; set; } = new List<Virus>();
        public List<Food> Foods { get; set; } = new List<Food>();
        public float LastMassUpdate { get; set; }
        public float SavedTotalGameTime { get; set; }
    }
}
