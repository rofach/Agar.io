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
        View camera;
        public static int sizeX, sizeY;
        public Game(int foodCount = 0, int botsCount = 0)
        {
            sizeX = 3000;
            sizeY = 3000;
            for (int i = 0; i < foodCount; i++)
            {
                Objects.Add(new Food(50));

            }
            var player = new Player();
            Objects.Add(player);
        }
        
       
        void Update(RenderWindow window)
        {
            Timer.Update();
            foreach (var cell in Objects.GetCells())
            {
                ICellsList cellList;
                cellList = cell as ICellsList;
                
                foreach (var obj in Objects.GetMoveblaObjects())
                {
                    foreach (var cl in cellList.GetCells)
                    {
                        //if (IsInViewZone(cl, camera))
                            //cl.Draw(window);
                        if (obj is Food food)
                            EatFood(cl, food, window);
                    }

                    if (obj is IDraw drawableObj && obj is Cell)
                    {
                        if (IsInViewZone(obj as Cell, camera))
                        {
                            drawableObj.Draw(window);
                        }
                    }

                }
                foreach (var cl in cellList.GetCells)
                {
                    if (IsInViewZone(cl, camera))
                        cl.Draw(window);
                }
            }
            Objects.MoveObj(window);
        }
        void EatFood(Cell obj, Food food, RenderWindow window)
        {
            float dist = GetDistance(obj, food);
            //if (dist < obj.Size + food.Size * 2)
            //{
            //    food.SetTarget(new Vector2f(obj.X, obj.Y));
            //    food.SetFlag(true);
            //}
            //else
            //{
            //    food.SetFlag(false);
            //}
            if (dist < obj.Size)
            {
                float prevRadius = obj.Size;
                obj.Mass += food.Mass;
                if (obj is PlayerCell)
                {
                    float radiusDifference = obj.Size - prevRadius;
                    float aspectRatio = window.Size.X / (float)window.Size.Y;
                    camera.Size += new Vector2f(radiusDifference * aspectRatio, radiusDifference);
                }

                food.ChangePos(sizeX, sizeY);
                food.SetFlag(false);
            }
        }
        private float GetDistance(Cell obj1, Cell obj2)
        {
            return (float)Math.Sqrt(Math.Pow(obj1.X - obj2.X, 2) + Math.Pow(obj1.Y - obj2.Y, 2));
        }
        private bool CanEat(Cell thisCell, Cell otherCell)
        {
            return GetDistance(thisCell, otherCell) < thisCell.Size;
        }
        private bool IsInViewZone(Cell gameObj, View camera)
        {
            return Math.Abs(camera.Center.X - gameObj.X) < camera.Size.X / 2 + gameObj.Size * 4 &&
                   Math.Abs(camera.Center.Y - gameObj.Y) < camera.Size.Y / 2 + gameObj.Size * 4;
        }
        private static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
        Vector2f GetCenterCamera()
        {
            int count = 0;
            Vector2f center = new Vector2f(0, 0);
            foreach (var obj in Objects.GetMoveblaObjects())
            {
                if (obj is Player player)
                {
                    foreach (var pl in player.cells)
                    {
                        count++;
                        center += new Vector2f(pl.X, pl.Y);
                    }
                }
            }
            return center / count;

        }

        public void Run()
        {
            var settings = new ContextSettings
            {
                AntialiasingLevel = 5
            };

            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Agario", Styles.Default, settings);
            window.Closed += new EventHandler(OnClose);
            camera = new View(new FloatRect(0, 0, 400, 300));
            window.SetView(camera);
            while (window.IsOpen)
            {
                window.Resized += (sender, e) =>
                {
                    FloatRect visibleArea = new FloatRect(0, 0, e.Width / 2, e.Height / 2);
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
