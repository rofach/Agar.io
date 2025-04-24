using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells
{
    class BotTeleport
        : Bot
    {
        public BotTeleport()
        {
            _cells.Add(new BotCell(0, 0, 100));
            _maxDivideCount = 2;
            _lastDivideTime = 0;
            _currentDivideCount = 0;
            _minMass = 200;
            
        }
        public override void SuperPower()
        {
            Random random = new Random();
            int x = random.Next(0, 1000);
            foreach (var cell in _cells)
            {

            }
        }
    }
}
