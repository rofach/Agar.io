using Agario.Interfaces;
using SFML.Graphics;

namespace Agario.Cells
{
    abstract class Bot : IMovable, IDrawable, ICellManager<Cell>
    {
        protected short _maxDivideCount;
        protected float _minMass;
        protected float _lastDivideTime;
        protected short _currentDivideCount;
        protected List<Cell> _cells = new List<Cell>();

        public List<Cell> Cells { 
            get { return _cells; }
            }

        public void AddCell(Cell cell)
        {
            _cells.Add(cell);
        }
        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
        }

        public void Draw(RenderWindow window)
        {
            foreach(var cell in _cells)
            {
                cell.Draw(window);
            }
        }

        public void Move(RenderWindow window)
        {
            foreach(var cell in _cells)
            {
                if (cell is IMovable moveCell)
                {
                    moveCell.Move(window);
                }
            }
        }

        
        public abstract void SuperPower();
    }
}
