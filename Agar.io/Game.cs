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
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.Index;



namespace Agario
{

    sealed public class Game
    {
        private View _camera;
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
            for(int i = 0; i < botsCount; i++)
            {
                Objects.Add(new BotTeleport());
            }

            for (int i = 0; i < 100; i++)
            {
                Objects.Add(new Virus());
            }
        }
        
       
        void Update(RenderWindow window)
        {
            Timer.Update();
            CheckVirusEating();
            CheckFoodEating(window);
            CheckCellEating();
            DrawObjects(window);
            MoveCells(window);
        }
        private void DrawObjects(RenderWindow window)
        {
            List<Cell> cellsToDraw = new List<Cell>();
            foreach (var obj in Objects.GetDrawableObjects())
            {
                if (obj is Food food)
                {
                    if (IsInViewZone(food, _camera))
                        food.Draw(window);
                }
                else if (obj is ICellManager<Cell> cellManager)
                {
                    foreach (var cell in cellManager.Cells)
                    {
                        if (IsInViewZone(cell, _camera))
                            cellsToDraw.Add(cell);
                    }
                }
                else if (obj is Virus virus)
                {
                    if (IsInViewZone(virus, _camera))
                        virus.Draw(window);
                }
            }
            cellsToDraw.Sort((a, b) => a.Radius.CompareTo(b.Radius));
            foreach (var cell in cellsToDraw)
            {
                cell.Draw(window);
            }
        }
        private void MoveCells(RenderWindow window)
        {
            foreach(var cell in Objects.GetMoveblaObjects())
            {
                cell.Move(window);
            }
        }
        private void CheckVirusEating()
        {
            var spatialIndex = new STRtree<Cell>();
            foreach (var cell in Objects.GetDrawableObjects().OfType<Virus>())
            {
                Envelope env = new Envelope(cell.X - cell.Radius,
                                            cell.X + cell.Radius,
                                            cell.Y - cell.Radius,
                                            cell.Y + cell.Radius);
                spatialIndex.Insert(env, cell);
            }
            foreach (var cellManagers in Objects.GetCells().OfType<IMulticellular>())
            {
                if (cellManagers is not IVirusSplittable splittableCell) continue;
                List<Cell> cells = new List<Cell>();
                foreach (var cell in cellManagers.Cells)
                {
                    float searchRange = 2 * cell.Radius;
                    Envelope searchEnv = new Envelope(
                        cell.X - searchRange, cell.X + searchRange,
                        cell.Y - searchRange, cell.Y + searchRange);

                    var nearbyViruses = spatialIndex.Query(searchEnv);
                    
                    foreach (var virus in nearbyViruses)
                    {
                        if(CanEat(cell, virus))
                        {
                            cell.Mass += virus.Mass;
                            //plCell.VirusSplit(cl);
                            cells.Add(cell);
                            Objects.Remove(virus);
                        }
                    }
                    
                }
                foreach (var cel in cells)
                {
                    splittableCell.VirusSplit(cel);
                }
            }
        }
        private void CheckFoodEating(RenderWindow window)
        {
            var spatialIndex = new STRtree<Cell>();
            foreach (var cell in Objects.GetMoveblaObjects().OfType<Cell>())
            {
                Envelope env = new Envelope(cell.X - cell.Radius,
                                            cell.X + cell.Radius,
                                            cell.Y - cell.Radius,
                                            cell.Y + cell.Radius);
                spatialIndex.Insert(env, cell);
            }

            spatialIndex.Build();

            foreach (var cell in Objects.GetCells().OfType<IMulticellular>())
            {
                foreach (var cl in cell.Cells)
                {
                    float searchRange = 50f + cl.Radius;
                    Envelope searchEnv = new Envelope(
                        cl.X - searchRange, cl.X + searchRange,
                        cl.Y - searchRange, cl.Y + searchRange);

                    var nearbyFoods = spatialIndex.Query(searchEnv);

                    foreach (var cellFood in nearbyFoods)
                    {
                        if (cellFood is Food food)
                        {
                            EatFood(cl, food, window);

                        }
                    }
                }
            }
        }
        private void CheckCellEating()
        {
            var allCells = Objects.GetMoveblaObjects().OfType<ICellManager<Cell>>()
                             .SelectMany(c => c.Cells).ToList();

            for (int i = 0; i < allCells.Count; i++)
            {
                for (int j = i + 1; j < allCells.Count; j++)
                {
                    var eater = allCells[i];
                    var victim = allCells[j];
                    if(eater is IMergeable cell1 && victim is IMergeable cell2)
                    {
                        if(cell1.ID == cell2.ID) continue;
                    }
                    if (CanEat(eater, victim) && eater.Radius > victim.Radius * 1.2f)
                    {
                        eater.Mass += victim.Mass;
                        Objects.RemoveCellFromAllManagers(victim);
                    }
                    else if (CanEat(victim, eater) && victim.Radius > eater.Radius * 1.2f)
                    {
                        victim.Mass += eater.Mass;
                        Objects.RemoveCellFromAllManagers(eater);
                    }
                }
            }
        }
        private void EatCell(Cell eater, Cell victim, RenderWindow window)
        {
            float dx = eater.X - victim.X;
            float dy = eater.Y - victim.Y;
            float distSq = dx * dx + dy * dy;
            if (distSq <= eater.Radius * eater.Radius && eater.Radius > victim.Radius)
            {
                eater.Mass += victim.Mass;
            }
        }
        private void EatFood(Cell cell, Food food, RenderWindow window)
        {
            float dist = GetDistance(cell, food);
            if (dist < cell.Radius + food.Radius * 4 && food.IsEaten == false)
            {
                food.Target = cell;
                food.IsEaten = true;
            }
            if (dist < cell.Radius)
            {
                float prevRadius = cell.Radius;
                cell.Mass += food.Mass;
                if (cell is PlayerCell)
                {
                    float radiusDifference = cell.Radius - prevRadius;
                    float aspectRatio = window.Size.X / (float)window.Size.Y;
                    _camera.Size += new Vector2f(radiusDifference * aspectRatio, radiusDifference);
                }

                food.ChangePos(sizeX, sizeY);
                food.IsEaten = false;
            }
        }
        private float GetDistance(Cell obj1, Cell obj2)
        {
            return (float)Math.Sqrt(Math.Pow(obj1.X - obj2.X, 2) + Math.Pow(obj1.Y - obj2.Y, 2));
        }
        private bool CanEat(Cell thisCell, Cell otherCell)
        {
            return GetDistance(thisCell, otherCell) < thisCell.Radius; //&& thisCell.Mass >= otherCell.Mass * 1.2f;
        }
        private bool IsInViewZone(Cell gameObj, View camera)
        {
            return Math.Abs(camera.Center.X - gameObj.X) < camera.Size.X / 2 + gameObj.Radius * 4 &&
                   Math.Abs(camera.Center.Y - gameObj.Y) < camera.Size.Y / 2 + gameObj.Radius * 4;
        }
        private static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
        private Vector2f GetCenterCamera()
        {
            int count = 0;
            Vector2f center = new Vector2f(0, 0);
            if (Objects.GetMoveblaObjects().Where(x => x is Player).Count() == 0)
            {
                return _camera.Center;
            }
            foreach (var obj in Objects.GetMoveblaObjects())
            {
                if (obj is Player player)
                {
                    foreach (var pl in player.Cells)
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
            _camera = new View(new FloatRect(0, 0, 400, 300));
            window.SetView(_camera);
            while (window.IsOpen)
            {
                window.Resized += (sender, e) =>
                {
                    FloatRect visibleArea = new FloatRect(0, 0, e.Width / 2, e.Height / 2);
                    _camera = new View(visibleArea);
                };
                _camera.Center = GetCenterCamera();              
                window.SetView(_camera);
                window.DispatchEvents();
                window.Clear(Color.White);
                Update(window);
                window.Display();
                
            }
        }
    }
}
