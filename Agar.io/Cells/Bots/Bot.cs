using Agario.Interfaces;
using Agario.Strategies;
using SFML.Graphics;
using SFML.System;
using System.Runtime.InteropServices.Marshalling;

namespace Agario.Cells.Bots
{
    abstract public class Bot : IUpdatable, IDrawable, IVirusSplittable, ICellManager<Cell>
    {
        protected short _maxDivideCount;
        protected float _minMass;
        protected float _lastDivideTime;
        protected short _currentDivideCount;
        protected List<Cell> _freeCells;
        protected List<Cell> _cells;
        protected Vector2f _targetPoint;
        protected IBehavior? _behavior;

        public Bot(int id)
        {
            ID = id;
            _cells = new List<Cell>();
            _freeCells = new List<Cell>();
            _targetPoint = new Vector2f(Objects.Random.Next(-Game.sizeX, Game.sizeX), Objects.Random.Next(-Game.sizeY, Game.sizeY));
            _lastDivideTime = 0;
            _currentDivideCount = 0;
            _maxDivideCount = 2;
            _minMass = 200;
            _behavior = new SafeBehavior();
                       
            _cells.Add(new PlayerCell(0, 0, _minMass, ID) { Position = new Vector2f(Objects.Random.Next(-Game.sizeX, Game.sizeX), Objects.Random.Next(-Game.sizeY, Game.sizeY)) });
            for (int i = 0; i < 6; i++)
            {
                _freeCells.Add(new PlayerCell(x: 0, y: 0, mass: 0, ID));
            }
        }
        public IBehavior? Behavior
        {
            get { return _behavior; }
            set { _behavior = value; }
        }
        public float LastDivideTime
        {
            get { return _lastDivideTime; }
            set { _lastDivideTime = value; }
        }
        public List<Cell> Cells => _cells;
        public List<Cell> FreeCells => _freeCells;
        public Vector2f TargetPoint => _targetPoint;
        public int ID { get; set; }
        public void AddCell(Cell cell)
        {
            _cells.Add(cell);
        }
        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
        }

        virtual public void Draw(RenderWindow window)
        {
            foreach (var cell in _cells)
            {
                cell.Draw(window);
            }
        }

        virtual public void Update(RenderWindow window)
        {
            _targetPoint = Behavior!.FindNewTargetPoint(this);
            foreach (var cell in _cells)
            {
                if (cell is PlayerCell moveCell)
                {
                    moveCell.Move(window, _targetPoint);
                }
            }
            Logic.HandleCollisions(_cells);
            Logic.Merge(_cells);
        }
        public abstract void SuperPower();
        public void VirusSplit(Cell cell)
        {
            int splitCount = Math.Min(4, 5 - _cells.Count);
            if (splitCount <= 0)
                return;

            float fragmentMass = cell.Mass / (2 * splitCount + 1);
            cell.Mass -= fragmentMass * splitCount;
            float angleStep = MathF.PI * 2f / splitCount;
            Vector2f center = new(cell.X, cell.Y);
            List<PlayerCell> newCells = _freeCells.Cast<PlayerCell>().Where(c => !_cells.Contains(c)).Take(splitCount).ToList();
            for (int i = 0; i < splitCount; i++)
            {
                float angle = i * angleStep;
                Vector2f dir = new Vector2f(cell.X + 1000 * MathF.Cos(angle), cell.Y + 1000 * MathF.Sin(angle));
                var cellToAdd = newCells[i];
                cellToAdd.Position = new Vector2f(cell.X + 100 * MathF.Cos(angle), cell.Y + 100 * MathF.Sin(angle));
                cellToAdd.Acceleration = true;
                cellToAdd.AccelerationDirection = dir;
                cellToAdd.AccelerationDistance = cell.Radius * 10;
                cellToAdd.StartAccelerationPoint = center;
                cellToAdd.Mass = fragmentMass;
                cellToAdd.DivisionTime = Timer.GameTime;
                _cells.Add(cellToAdd);
            }
        }

        public Cell GetSplitCell(Cell cell, float newMass, float x, float y, float currentTime)
        {
            if (cell is not PlayerCell playerCell)
                throw new ArgumentException("Cell is not a PlayerCell", nameof(cell));
            PlayerCell? newCell = _freeCells.Cast<PlayerCell>().First(c => !_cells.Contains(c));
            if (newCell == null) throw new InvalidOperationException("No free cells available");
            newCell.InitializeCircle(x, y, newMass);
            newCell.Acceleration = true;
            newCell.DivisionTime = currentTime;
            newCell.AccelerationDirection = playerCell.Direction * 10000;
            newCell.AccelerationDistance = playerCell.Radius * 6; // ПОДУМАТИ ЯК КРАЩЕ
            newCell.StartAccelerationPoint = new Vector2f(cell.X, cell.Y);
            return newCell;
        }

       
        
    }
}
