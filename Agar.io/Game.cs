using Agario.Cells;
using Agario.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Agario
{

    public class Game
    {
        List<IDraw> objects;
        List<IMove> movableObjects;
        Thread drawThread;
        Player player;
        View camera;
        public static int sizeX, sizeY;
        public Game(int foodCount = 0, int botsCount = 0)
        {
            objects = new List<IDraw>();
            movableObjects = new List<IMove>();
            sizeX = 3000;
            sizeY = 3000;
            for (int i = 0; i < foodCount; i++)
            {
                Objects.Add(new Food(50));
                
            }
            player = new Player();

            Objects.Add(player);

        }
        void EatFood(Cell obj, Food food, RenderWindow window)
        {
            float dist = GetDistance(obj, food);

            if (dist < obj.Size + food.Size * 2)
            {
                food.SetTarget(new Vector2f(obj.X, obj.Y));
                food.SetFlag(true);
            }
            else
            {
                food.SetFlag(false);
            }
            if (dist < obj.Size)
            {
                float prevRadius = obj.Size;
                obj.Mass += food.Mass;
                if (obj is Player)
                {
                    float radiusDifference = obj.Size - prevRadius;
                    float aspectRatio = window.Size.X / (float)window.Size.Y;
                    camera.Size += new Vector2f(radiusDifference * aspectRatio, radiusDifference);
                }
            
                food.ChangePos(sizeX, sizeY);
                food.SetFlag(false);
            }
        }
        Vector2f GetCenterCamera()
        {
            int count = 0;
            Vector2f center = new Vector2f(0, 0);
            foreach (var obj in Objects.GetMoveblaObjects())
            {
                if (obj is Player)
                {
                    count++;
                    center += new Vector2f(obj.X, obj.Y);
                }
            }
            return center / count;

        }
        void Update(RenderWindow window)
        {
            Timer.Update();

            foreach (var cell in Objects.GetCells())
            {
                foreach (var obj in Objects.GetDrawableObjects())
                {
                    if (IsInViewZone(obj, camera))
                    {
                        obj.Draw(window);
                        if (obj is Food food && cell is Player player)
                            EatFood(player, food, window);
                    }
                }
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && Timer.GameTime - player.lastDivideTime > 1)
            {
                var list = Objects.GetCells().ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var cell = list[i];
                    if (cell is Player)
                        ((Player)cell).Divide();
                }
            }
            Objects.MoveObj(window);
        }
        private float GetDistance(IDraw obj1, IDraw obj2)
        {
            return (float)Math.Sqrt(Math.Pow(obj1.X - obj2.X, 2) + Math.Pow(obj1.Y - obj2.Y, 2));
        }
        private bool CanEat(Cell thisCell, Cell otherCell)
        {
            return GetDistance(thisCell, otherCell) < thisCell.Size;
        }
        private bool IsInViewZone(IDraw gameObj, View camera)
        {
            return Math.Abs(camera.Center.X - gameObj.X) < camera.Size.X / 2 + gameObj.Size * 4 &&
                   Math.Abs(camera.Center.Y - gameObj.Y) < camera.Size.Y / 2 + gameObj.Size * 4;
        }
        private static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        public void Run()
        {
            var settings = new ContextSettings
            {
                AntialiasingLevel = 5
            };

            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Agario", Styles.Default, settings);
            window.Closed += new EventHandler(OnClose);
            camera = new View(new FloatRect(player.X, player.Y, 400, 300));
            window.SetView(camera);
            
            while (window.IsOpen)
            {
                window.Resized += (sender, e) =>
                {
                    FloatRect visibleArea = new FloatRect(player.X, player.Y, e.Width / 2, e.Height / 2);
                    camera = new View(visibleArea);
                };
                camera.Center = GetCenterCamera();
                window.SetView(camera);
                window.DispatchEvents();
                window.Clear(Color.White);
                Update(window);
                window.Display();
                
            }
        }



    }
}
