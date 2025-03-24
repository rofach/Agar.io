using Agario.Cells;
using Agario.Interfaces;
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
        static List<IDraw> _drawableObjects = new();
        static List<IMove> _movableObjects = new();
        static List<IMove> _cells = new();

        static public void Add<T>(T obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj is IMove)
            {
                _movableObjects.Add(obj as IMove);
                if(obj is Player)
                    _cells.Add(obj as Player);
            }
            if (obj is IDraw)
                _drawableObjects.Add(obj as IDraw);
            else
                throw new ArgumentException("Not is a game object");
        }
        
        static public void MoveObj(RenderWindow window)
        {
            foreach (IMove obj in _movableObjects)
            {
                obj.Move(window);
                
            }
        }
        
        static public Texture texture = new Texture("textures/text1.png");
        static public IEnumerable<IDraw> GetDrawableObjects() => _drawableObjects;
        static public IEnumerable<IDraw> GetMoveblaObjects() => _movableObjects;
        static public IEnumerable<IMove> GetCells() => _cells;

    }
}
