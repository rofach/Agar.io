using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Agario.Interfaces;

namespace Agario.Cells
{
    sealed class Player : IMovable, IDrawable, IVirusSplittable, ICellManager<Cell>
    {       
        private Texture _texture = new Texture("textures/test2.jpg");
        private int _maxCellsCount = 16;
        private int _cellsCount;
        private float _minMass = 200;
        private float _lastDivideTime = 0;
        private List<Cell> _cells = new List<Cell>();
        private int id = 1;
        public Player()
        {
            _cells.Add(new PlayerCell(mass: 200, id: 1));
            _cellsCount = _cells.Count;
        }
        public List<Cell> Cells
        {
            get { return _cells; }           
        }
        public void VirusSplit(Cell cell)
        {
            int splitCount = Math.Min(8, _maxCellsCount - _cells.Count);
            if (splitCount <= 0)
                return;

            float fragmentMass = cell.Mass / (2 * splitCount + 1);
            cell.Mass -= fragmentMass * splitCount;
            float angleStep = (MathF.PI * 2f) / splitCount;   
            Vector2f center = new(cell.X, cell.Y);
            for (int i = 0; i < splitCount; i++)
            {
                float angle = i * angleStep;
                
                Vector2f dir = new Vector2f(cell.X + 1000 * MathF.Cos(angle), cell.Y + 1000 * MathF.Sin(angle));
                var newCell = new PlayerCell(cell.X + 10 * MathF.Cos(angle), cell.Y + 10 * MathF.Sin(angle), fragmentMass, id)
                {
                    Acceleration = true,
                    DivisionTime = Timer.GameTime,
                    AccelerationDirection = dir,
                    AccelerationDistance = cell.Radius*10,
                    StartAccelerationPoint = center,
                };
                _cells.Add(newCell);
            }

            

        }
        public void Move(RenderWindow window)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && Timer.GameTime - _lastDivideTime > 0.3)
            {
                _lastDivideTime = Timer.DeltaTime;
                Logic.Divide(_cells, _maxCellsCount,ref _lastDivideTime, _minMass);
            }
            Logic.Merge(_cells);
            foreach (var cell in _cells)
            {
                if (cell is IMovable playerCell)
                    playerCell.Move(window);
            }
            Logic.HandleCollisions(_cells);
        }

        public void Draw(RenderWindow window)
        {
            foreach (var cell in _cells)
            {
                cell.Draw(window);
            }
        }

        public void AddCell(Cell cell)
        {
            _cells.Add(cell);
        }

        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
        }

        public float GetTotalMass() => _cells.Sum(cell => cell.Mass);

    }
}
