using Agario.Interfaces;
using Agario.Strategies;
using SFML.Graphics;
using SFML.System;
using System.Runtime.InteropServices.Marshalling;

namespace Agario.Cells
{
    abstract public class Bot : IMovable, IDrawable, ICellManager<Cell>
    {
        protected short _maxDivideCount;
        protected float _minMass;
        protected float _lastDivideTime;
        protected short _currentDivideCount;
        protected List<Cell> _freeCells = new List<Cell>();
        protected List<Cell> _cells = new List<Cell>();
        protected Vector2f _targetPoint;
        protected IStrategy? _strategy;
        public IStrategy? Strategy
        {
            get { return _strategy; }
            set { _strategy = value; }
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
            foreach(var cell in _cells)
            {
                cell.Draw(window);
            }
        }

        virtual public void Move(RenderWindow window)
        {
            if(_cells.Count > 1)
                Strategy = new SafeBehavior();
            else
                Strategy = new AggressiveBehavior();
            _targetPoint = Strategy.FindNewTargetPoint(this);
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
