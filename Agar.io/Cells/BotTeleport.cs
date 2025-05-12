
using Agario.Interfaces;
using Agario.Strategies;
using SFML.Graphics;
using SFML.System;

namespace Agario.Cells
{
    class BotTeleport
        : Bot, IVirusSplittable
    {
        public BotTeleport(int id)
        {
            ID = id;
            _cells.Add(new PlayerCell(0, 0, 100, ID) { Position = new Vector2f(Objects.Random.Next(-Game.sizeX, Game.sizeX), Objects.Random.Next(-Game.sizeY, Game.sizeY))});
            _maxDivideCount = 2;
            _lastDivideTime = 0;
            _currentDivideCount = 0;
            _minMass = 200;
            _targetPoint = new Vector2f(Objects.Random.Next(-Game.sizeX, Game.sizeX), Objects.Random.Next(-Game.sizeY, Game.sizeY));
            
            for (int i = 0; i < 16; i++)
            {
                _freeCells.Add(new PlayerCell(x: 0, y: 0, mass: 0, ID));
            }
            Strategy = new SafeBehavior();
        }
        public override void SuperPower()
        {

        }
        public void VirusSplit(Cell cell)
        {
            int splitCount = Math.Min(5, 5 - _cells.Count);
            if (splitCount <= 0)
                return;

            float fragmentMass = cell.Mass / (2 * splitCount + 1);
            cell.Mass -= fragmentMass * splitCount;
            float angleStep = (MathF.PI * 2f) / splitCount;
            Vector2f center = new(cell.X, cell.Y);
            List<PlayerCell> newCells = _freeCells.Cast<PlayerCell>().Where(c => !_cells.Contains(c)).Take(splitCount).ToList();
            for (int i = 0; i < splitCount; i++)
            {
                float angle = i * angleStep;
                Vector2f dir = new Vector2f(cell.X + 1000 * MathF.Cos(angle), cell.Y + 1000 * MathF.Sin(angle));
                var cellToAdd = newCells[i];
                cellToAdd.Position = new Vector2f(cell.X + 10 * MathF.Cos(angle), cell.Y + 10 * MathF.Sin(angle));
                cellToAdd.Acceleration = true;
                cellToAdd.AccelerationDirection = dir;
                cellToAdd.AccelerationDistance = cell.Radius * 10;
                cellToAdd.StartAccelerationPoint = center;
                cellToAdd.Mass = fragmentMass;
                cellToAdd.DivisionTime = Timer.GameTime;
                _cells.Add(cellToAdd);
            }
        }

        /*public override void Move(RenderWindow window)
        {
            _targetPoint = Strategy.FindNewTargetPoint(_cells, _targetPoint);
            foreach (var cell in _cells)
            {
                if (cell is PlayerCell moveCell)
                {
                    moveCell.Move(window, _targetPoint);
                }
            }
            Logic.HandleCollisions(_cells);
        }*/
    }
}
