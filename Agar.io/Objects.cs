using Agario.Cells;
using Agario.Interfaces;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Agario
{
    static class Objects
    {
        private static List<IDrawable> _drawableObjects = new();
        private static List<IMovable> _movableObjects = new();
        private static List<ICellManager<Cell>> _cellManagers = new();
        private static STRtree<Cell> _cellsTree = new();
        private static STRtree<Cell> _foodTree = new();
        static public Random Random = new Random();
        public static void Add<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is IMovable)
            {
                _movableObjects.Add(obj as IMovable);
                if(obj is ICellManager<Cell> cell)
                    _cellManagers.Add(cell);
            }
            if (obj is IDrawable)
                _drawableObjects.Add(obj as IDrawable);
            else
                throw new ArgumentException("Not is a game object");
        }

        public static void MoveObj(RenderWindow window)
        {

            foreach (IMovable obj in _movableObjects)
            {
                obj.Move(window);
            }
        }
        public static void Remove<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is IMovable)
            {
                _movableObjects.Remove(obj as IMovable);
                if (obj is ICellManager<Cell> cell)
                    _cellManagers.Remove(cell);
            }
            if (obj is IDrawable)
                _drawableObjects.Remove(obj as IDrawable);
            else
                throw new ArgumentException("Not is a game object");
        }
        public static void RemoveCellFromAllManagers(Cell cell)
        {
            List<ICellManager<Cell>> cellsToRemove = new();
            foreach (var obj in _cellManagers.OfType<ICellManager<Cell>>())
            {
                
                obj.RemoveCell(cell);
                if (obj.Cells.Count == 0)
                {
                    cellsToRemove.Add(obj);
                }

            }
            foreach(var obj in cellsToRemove)
            {
                Remove(obj);
            }
        }
        public static void UpdateObjects()
        {
            _cellsTree = new STRtree<Cell>();
            foreach (var cell in _cellManagers.SelectMany(x => x.Cells))
            {
                Envelope env = new Envelope(cell.X - cell.Radius,
                                            cell.X + cell.Radius,
                                            cell.Y - cell.Radius,
                                            cell.Y + cell.Radius);
                _cellsTree.Insert(env, cell);
            }
            _cellsTree.Build();

            _foodTree = new STRtree<Cell>();
            foreach (Food cell in _drawableObjects.Where(d => d is Food).ToList())
            {
                Envelope env = new Envelope(cell.X - cell.Radius,
                                            cell.X + cell.Radius,
                                            cell.Y - cell.Radius,
                                            cell.Y + cell.Radius);
                _foodTree.Insert(env, cell);
            }
            _foodTree.Build();
        }

        public static Texture texture = new Texture("textures/text1.png");
        public static IEnumerable<IDrawable> GetDrawableObjects() => _drawableObjects;
        public static IEnumerable<IMovable> GetMoveblaObjects() => _movableObjects;
        public static IEnumerable<ICellManager<Cell>> GetCells() => _cellManagers;
        public static STRtree<Cell> GetCellsTree() => _cellsTree;
        public static STRtree<Cell> GetFoodTree() => _foodTree;

    }

}
