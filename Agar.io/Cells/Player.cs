using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Agario.Interfaces;
using System.Security.Principal;
using System.Numerics;

namespace Agario.Cells
{
    sealed class Player : IMove, IDraw, IMulticellular
    {       
        private Texture _texture = new Texture("textures/test2.jpg");
        private short _maxDivideCount = 16;
        private float _minMass = 200;
        private float _lastDivideTime = 0;
        private short _currentDivideCount = 0;
        public List<Cell> cells = new List<Cell>();


        public Player()
        {
            cells.Add(new PlayerCell());
        }
        public List<Cell> GetCells
        {
            get { return cells; }
            set { cells = value; }
        }
        public void Divide()
        {
            cells.Sort((a, b) => b.Mass.CompareTo(a.Mass));
            int count = cells.Count;    
            List<Cell> cellsToAdd = new List<Cell>();
            foreach (PlayerCell cell in cells)
            {
                if (_currentDivideCount < _maxDivideCount && cell.Mass >= 2 * _minMass)
                {
                    cell.Mass /= 2;
                    PlayerCell newCell = new PlayerCell(cell.X - cell.Size * 2, cell.Y - cell.Size * 2, cell.Mass);
                    newCell.Acceleration = true;
                    newCell.DivisionTime = Timer.GameTime;
                    newCell.AccelerationDirection = cell.Direction * 10000000;
                    _lastDivideTime = Timer.GameTime;
                    cellsToAdd.Add(newCell);
                    _currentDivideCount++;
                }
            }
            cells.AddRange(cellsToAdd);
        }
        public void Unite()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (((PlayerCell)cells[i]).IsMergeable)
                {
                    for (int j = i + 1; j < cells.Count; j++)
                    {
                        if (!((PlayerCell)cells[j]).IsMergeable)
                            continue;
                        if (i >= cells.Count || j >= cells.Count) break;
                        if (Logic.CanEat(cells[i], cells[j]) && cells[i] != cells[j])
                        {
                            cells[i].Mass += cells[j].Mass;
                            cells.Remove(cells[j]);
                            _currentDivideCount--;
                        }
                        
                    }
                }
            }
            
        }
        private void HandleCollisions()
        {

            Vector2f[] correctionVectors = new Vector2f[cells.Count];
            for (int i = 0; i < cells.Count; i++)
            {
                correctionVectors[i] = new Vector2f(0, 0);
            }

            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = i + 1; j < cells.Count; j++)
                {
                    if (!(cells[i] is PlayerCell pc1 && cells[j] is PlayerCell pc2))
                        continue;

                    if (pc1.IsMergeable && pc2.IsMergeable || (pc1.Acceleration || pc2.Acceleration))
                        continue;

                    Vector2f pos1 = pc1.Position;
                    Vector2f pos2 = pc2.Position;
                    Vector2f delta = pos2 - pos1;
                    float actualDistance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    float minDistance = cells[i].Size + cells[j].Size + 8;

                    if (actualDistance == 0)
                        continue;

                    if (actualDistance < minDistance)
                    {
                        float overlap = (minDistance - actualDistance) / 2;
                        Vector2f normal = new Vector2f(delta.X / actualDistance, delta.Y / actualDistance);
                        correctionVectors[i] -= normal * overlap;
                        correctionVectors[j] += normal * overlap;
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is PlayerCell playerCell)
                {
                    playerCell.Circle.Position += correctionVectors[i] * 0.5f;

                    playerCell.Circle.Position = new Vector2f(
                        Math.Clamp(playerCell.Circle.Position.X, -Game.sizeX, Game.sizeX),
                        Math.Clamp(playerCell.Circle.Position.Y, -Game.sizeY, Game.sizeY)
                    );
                }
            }
        }
        public void Move(RenderWindow window)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && Timer.GameTime - _lastDivideTime > 0.3)
            {
                _lastDivideTime = Timer.DeltaTime;
                Divide();

            }

            Unite();
            foreach (var cell in cells)
            {
                if (cell is PlayerCell playerCell)
                    playerCell.Move(window);
            }
            HandleCollisions();
            
        }

        public void Draw(RenderWindow window)
        {
            foreach (var cell in cells)
            {
                cell.Draw(window);
            }
        }
    }
}
