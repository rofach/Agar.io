using Agario.GameLogic;
using Agario.Strategies;
using NetTopologySuite.Geometries;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = Agario.GameLogic.Timer;

namespace Agario.Cells.Bots
{
    public class SpeedBot : Bot
    {
        private float lastSuperPowerUsingTime;
        public SpeedBot(int id) : base(id)
        {
            Behavior = new AggressiveBehavior();
        }
        public override void UseSuperPower()
        {
            if (Timer.GameTime - lastSuperPowerUsingTime < 60)
                return;
            foreach (PlayerCell cell in Cells)
            {
                cell.Acceleration = true;
                cell.AccelerationDistance = cell.Radius * 10;
                cell.AccelerationDirection = _targetPoint * 10;
                cell.StartAccelerationPoint = new Vector2f(cell.X, cell.Y);
                cell.AccelerationTime = Timer.GameTime;
            }
            lastSuperPowerUsingTime = Timer.GameTime;
        }
    }
}
