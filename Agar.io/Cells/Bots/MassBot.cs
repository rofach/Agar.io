using Agario.Strategies;
using NetTopologySuite.Geometries;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.Cells.Bots
{
    public class MassBot : Bot
    {
        private float _tempMass;
        private float _lastSuperPowerUsingTime;
        private Cell? _cellUsedPower;
        public MassBot(int id) : base(id)
        {
            Behavior = new SafeBehavior();
        }

        public override void SuperPower()
        {
            if(Timer.GameTime - _lastSuperPowerUsingTime < 60 || _cellUsedPower != null)
                return;
            var allCells = Objects.GetCellsTree();
            //float tempMass = 0;
            foreach (var myCell in Cells)
            {
                float range = myCell.Radius * 2;
                var env = new Envelope(
                    myCell.Position.X - range, myCell.Position.X + range,
                    myCell.Position.Y - range, myCell.Position.Y + range);

                var nearby = allCells.Query(env)
                                    .OfType<PlayerCell>()
                                    .Where(e => e != myCell);
                bool used = false;
                foreach (var enemy in nearby)
                {                  
                    foreach (var cell in Cells)
                    {
                        if (Logic.GetDistanceBetweenCells(enemy, cell) < cell.Radius * 3 && enemy.Mass > cell.Mass * 1.2)
                        {
                            _tempMass = enemy.Mass - cell.Mass;
                            cell.Mass = enemy.Mass;
                            _cellUsedPower = cell;
                            used = true;
                            _lastSuperPowerUsingTime = Timer.GameTime;
                            break;
                        }
                    }
                    if(used) break;
                }
                if (used) break;
            }
        }

        public override void Update(RenderWindow window)
        {
            if (Timer.GameTime - _lastSuperPowerUsingTime > 5)
            {
                if (_cellUsedPower != null)
                {
                    if (!Cells.Contains(_cellUsedPower))
                    {
                        _cellUsedPower = null;
                    }
                    else
                    {
                        _cellUsedPower.Mass -= Math.Min(_tempMass, _cellUsedPower.Mass / 2);
                        _cellUsedPower = null;
                    }
                }
            }
            base.Update(window);
        }
    }
}
