using Agario.GameLogic;
using Agario.GameLogic;
using Agario.Strategies;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using System.Runtime.InteropServices.Marshalling;

namespace Agario.Cells.Bots
{
    [JsonObject(MemberSerialization.OptIn, IsReference = true)]
    abstract public class Bot : IUpdatable, IVirusSplittable, ICellManager<Cell>
    {
        [JsonProperty]
        protected short _maxDivideCount;
        [JsonProperty]
        protected short _maxCount;
        [JsonProperty]
        protected float _minMass;
        [JsonProperty]
        protected float _lastDivideTime;
        [JsonProperty]
        protected short _currentDivideCount;
        [JsonProperty]
        protected List<Cell> _freeCells;
        [JsonProperty]
        protected List<Cell> _cells;
        [JsonProperty]
        protected Vector2f _targetPoint;
        [JsonProperty]
        protected IBehavior? _behavior;
        public Bot(int id)
        {
            ID = id;
            _cells = new List<Cell>();
            _freeCells = new List<Cell>();
            _lastDivideTime = 0;
            _currentDivideCount = 0;
            _maxDivideCount = 2; _maxCount = 6;
            _minMass = 200;
            _targetPoint = new Vector2f(Game.Random.Next(-Game.MapSizeX, Game.MapSizeX), Game.Random.Next(-Game.MapSizeY, Game.MapSizeY));
            _cells.Add(new PlayerCell(0, 0, _minMass, ID)
            {
                TimeToMerge = 10,
                Position = new Vector2f(Game.Random.Next(-Game.MapSizeX, Game.MapSizeX), Game.Random.Next(-Game.MapSizeY, Game.MapSizeY))
            });
            for (int i = 0; i < _maxCount; i++)
            {
                _freeCells.Add(new PlayerCell(x: 0, y: 0, mass: _minMass, ID) 
                { 
                    TimeToMerge = 10, CellColor = ((PlayerCell)_cells[0]).CellColor 
                });
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
            Objects.Add(cell);
        }
        public void RemoveCell(Cell cell)
        {
            _cells.Remove(cell);
            Objects.Remove(cell);
        }
        virtual public void Update(RenderWindow window)
        {
            _targetPoint = Behavior!.FindNewTargetPoint(this);
            foreach (var cell in _cells)
            {
                if (cell is PlayerCell moveCell)
                {
                    moveCell.Move(_targetPoint);
                }
            }
            Logic.HandleCollisions(_cells);
            Logic.Merge(this);
        }
        public abstract void UseSuperPower();
        public void VirusSplit(Cell cell)
        {
            int splitCount = Math.Min(_maxCount - 1, _maxCount - _cells.Count);
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
                cellToAdd.AccelerationDistance = cell.Radius * 5;
                cellToAdd.StartAccelerationPoint = center;
                cellToAdd.Mass = fragmentMass;
                cellToAdd.DivisionTime = GameLogic.Timer.GameTime;
                AddCell(cellToAdd);
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
            newCell.AccelerationDistance = playerCell.Radius * 3 + 300f; 
            newCell.StartAccelerationPoint = cell.Position;
            return newCell;
        }

       
        
    }
}
