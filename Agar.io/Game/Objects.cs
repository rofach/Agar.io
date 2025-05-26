using Agario.Cells;
using Agario.GameLogic;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Agario.GameLogic
{
    static class Objects
    {
        private static List<IDrawable> _drawableObjects = new();
        private static List<IUpdatable> _updatableObjects = new();
        private static List<ICellManager<Cell>> _cellManagers = new();
        private static STRtree<Cell> _cellsTree = new();
        private static STRtree<Cell> _foodTree = new();
        static int k = 0;
       
        public static void Add<T>(T obj)
        {
            bool succes = false;
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is IUpdatable)
            {
                _updatableObjects.Add(obj as IUpdatable);
                succes = true;
            }
            if (obj is ICellManager<Cell> manager)
            {
                _cellManagers.Add(manager);
                _drawableObjects.AddRange(manager.Cells.Where(c => c is IDrawable));
                succes = true;
            }
            if (obj is IDrawable)
            {
                _drawableObjects.Add(obj as IDrawable);
                succes = true;
            }
            if(!succes)
                throw new ArgumentException("Cannot add this object");
        }     
        public static void Remove<T>(T obj)
        {
            bool succes = false;
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is IUpdatable)
            {
                _updatableObjects.Remove(obj as IUpdatable);
                succes = true;
            }
            if (obj is ICellManager<Cell> manager)
            {
                foreach (var cell in manager.Cells.Where(c => c is IDrawable))
                {
                    _drawableObjects.Remove(cell);
                }
                _cellManagers.Remove(manager);
                succes = true;
            }
            if (obj is IDrawable)
            {
                _drawableObjects.Remove(obj as IDrawable);
                succes = true;
            }
            if(!succes)
                throw new ArgumentException("Cannot remove this object");
        }
        public static void RemoveCellFromAllManagers(Cell cell)
        {
            List<ICellManager<Cell>> cellsToRemove = new();
            foreach (var obj in _cellManagers.OfType<ICellManager<Cell>>())
            {
                obj.RemoveCell(cell);
                if (obj.Cells.Count == 0)
                    cellsToRemove.Add(obj);
            }
            foreach (var obj in cellsToRemove)
                Remove(obj);
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
        public static IEnumerable<IDrawable> GetDrawableObjects() => _drawableObjects;
        public static IEnumerable<IUpdatable> GetMoveblaObjects() => _updatableObjects;
        public static IEnumerable<ICellManager<Cell>> GetCellsManagers() => _cellManagers;
        public static STRtree<Cell> GetCellsTree() => _cellsTree;
        public static STRtree<Cell> GetFoodTree() => _foodTree;

        public static void ClearAll()
        {
            _drawableObjects.Clear();
            _updatableObjects.Clear();
            _cellManagers.Clear();
            _cellsTree = new STRtree<Cell>();
            _foodTree = new STRtree<Cell>();

        }
    }

}
