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
    class Player : IMove, IDraw, ICellsList
    {

        private float speed;
        private Vector2f direction;
        private Vector2f position;
        public float lastDivideTime;
        Texture texture = new Texture("textures/test2.jpg");
        short maxD = 2;
        public short currentD = 0;
        public bool ass;
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

        }

        public void Unite()
        {

        }
        public void Move(RenderWindow window)
        {
            foreach (var cell in cells)
            {
                if (cell is PlayerCell playerCell)
                    playerCell.Move(window);
            }
        }

        public void Draw(RenderWindow window)
        {
            foreach (var cell in cells)
            {
                window.Draw(cell.Circle);
            }
        }
    }
}
