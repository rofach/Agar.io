using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Agario.GameLogic;
using Timer = Agario.GameLogic.Timer;
using Newtonsoft.Json;

namespace Agario.Cells
{
    [JsonObject(MemberSerialization.OptIn, IsReference = true)]
    sealed public class Player : IUpdatable, IVirusSplittable, ICellManager<Cell>
    {
        [JsonProperty]
        private int _maxCellsCount;
        [JsonProperty]
        private float _minMass;
        [JsonProperty]
        private float _lastDivideTime;
        [JsonProperty]
        private List<Cell> _cells;
        [JsonProperty]
        private List<Cell> _freeCells;
        [JsonProperty]
        private int _id;
        [JsonProperty]
        private Color _cellsColor;
        public Player(int id)
        {
            _id = id;
            _maxCellsCount = 16;
            _minMass = 200;
            _lastDivideTime = 0;
            _freeCells = new List<Cell>();
            _cells = new List<Cell>();
            _cells.Add(new PlayerCell(mass: _minMass, id: _id) { TextToDraw = "You"});
            _cellsColor = ((PlayerCell)_cells[0]).CellColor;
            for (int i = 0; i < _maxCellsCount; i++)
            {
                _freeCells.Add(new PlayerCell(x: 0, y: 0, mass: 1, id) { CellColor = _cellsColor, TextToDraw = "You" });
            }
        }
        public List<Cell> Cells
        {
            get { return _cells; }           
        }
        public List<Cell> FreeCells
        {
            get { return _freeCells; }
        }
        public int ID { get => _id; set => _id = value; }

        public void VirusSplit(Cell cell)
        {
            int splitCount = Math.Min(8, _maxCellsCount - _cells.Count);
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
                cellToAdd.Position = new Vector2f(cell.X + cell.Radius * MathF.Cos(angle), cell.Y + cell.Radius * MathF.Sin(angle));
                cellToAdd.Acceleration = true;
                cellToAdd.AccelerationDirection = dir;
                cellToAdd.AccelerationDistance = cell.Radius * 5;
                cellToAdd.StartAccelerationPoint = center;
                cellToAdd.Mass = fragmentMass;
                cellToAdd.DivisionTime = Timer.GameTime;
                AddCell(cellToAdd);
            }
            
        }
        public void SplitKeyPressed()
        {
            Logic.Divide(this, _maxCellsCount, ref _lastDivideTime, _minMass);
        }
        public void Update(RenderWindow window)
        {
            Logic.Merge(this);
            var targetPoint = window.MapPixelToCoords(Mouse.GetPosition(window));
            foreach (var cell in _cells)
            {
                if (cell is PlayerCell playerCell)
                    playerCell.Move(targetPoint);
            }
            Logic.HandleCollisions(_cells);
        }
        public void AddCell(Cell cell)
        {
            _cells.Add(cell);
            Objects.Add(cell);
        }

        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
            Objects.Remove(cell);
        }

        public float GetTotalMass() => _cells.Sum(cell => cell.Mass);

        public Cell GetSplitCell(Cell cell, float newMass, float x, float y, float currentTime)
        {
            if(cell is not PlayerCell playerCell)
                throw new ArgumentException("Cell is not a PlayerCell", nameof(cell));
            PlayerCell? newCell = _freeCells.Cast<PlayerCell>().First(c => !_cells.Contains(c));
            if(newCell == null) throw new InvalidOperationException("No free cells available");
            newCell.InitializeCircle(x, y, newMass);
            newCell.Acceleration = true;
            newCell.DivisionTime = currentTime;
            newCell.AccelerationDirection = playerCell.Direction * 10000;
            newCell.AccelerationDistance = playerCell.Radius*3 + 300f;
            newCell.StartAccelerationPoint = new Vector2f(cell.X, cell.Y);
            return newCell;
        }
    }
}
