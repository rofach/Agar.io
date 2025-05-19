using Agario.Cells;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using Agario.Strategies;
using Agario.Cells.Bots;
using Agario.GameLogic;

namespace Agario.GameLogic
{
    sealed public class Game
    {
        public static int MapSizeX = 4000, MapSizeY = 4000;
        private float _lastMassUpdate;
        private View? _camera;
        private Player? _player;
        private BotBehaviorsManager? _botStrategiesManager;
        private VertexArray? _grid;
        static public readonly Random Random = new Random();
        public Game(int foodCount = 0, int botsCount = 0, int virusCount = 0)
        {
            _lastMassUpdate = 0;
            for (int i = 0; i < foodCount; i++)
            {
                Objects.Add(new Food(50));

            }
            _player = new Player(1000);
            Objects.Add(_player);
            for (int i = 0; i < botsCount; i++)
            {
                if (i % 3 == 0)
                    Objects.Add(new TeleportBot(i));
                else if (i % 3 == 1)
                    Objects.Add(new SpeedBot(i));
                else
                {
                    Objects.Add(new MassBot(i));
                }
            }
            for (int i = 0; i < virusCount; i++)
            {
                Objects.Add(new Virus(1000));
            }

            _botStrategiesManager = new BotBehaviorsManager();
            CreateGrid();

        }
        private void Draw(RenderWindow window)
        {
            DrawGrid(window);
            DrawObjects(window);
        }
        private void Update(RenderWindow window)
        {
            Timer.Update();
            _botStrategiesManager!.UpdateBehavior();
            CheckVirusEating();
            CheckFoodEating(window);
            CheckCellEating();
            UpdateCells(window);
        }
        private void DrawObjects(RenderWindow window)
        {
            List<Cell> cellsToDraw = new List<Cell>();
            foreach (var obj in Objects.GetDrawableObjects())
            {
                if (obj is Cell cell)
                {
                    if (IsInViewZone(obj, _camera))
                        cellsToDraw.Add(cell);
                }
                else obj.Draw(window);
            }
            cellsToDraw.Sort((a, b) => a.Radius.CompareTo(b.Radius));
            foreach (var cell in cellsToDraw)
            {
                cell.Draw(window);
            }
        }
        private void DrawGrid(RenderWindow window)
        {
            window.Draw(_grid);
        }
        private void CreateGrid()
        {
            float thickness = 0.5f;
            float spacing = 30f;
            var color = new Color(0, 0, 0, 50);

            _grid = new VertexArray(PrimitiveType.Quads);

            for (float x = -MapSizeX; x <= MapSizeX; x += spacing)
            {
                _grid.Append(new Vertex(new Vector2f(x - thickness, -MapSizeY), color));
                _grid.Append(new Vertex(new Vector2f(x + thickness, -MapSizeY), color));
                _grid.Append(new Vertex(new Vector2f(x + thickness, +MapSizeY), color));
                _grid.Append(new Vertex(new Vector2f(x - thickness, +MapSizeY), color));
            }

            for (float y = -MapSizeY; y <= MapSizeY; y += spacing)
            {
                _grid.Append(new Vertex(new Vector2f(-MapSizeX, y - thickness), color));
                _grid.Append(new Vertex(new Vector2f(+MapSizeX, y - thickness), color));
                _grid.Append(new Vertex(new Vector2f(+MapSizeX, y + thickness), color));
                _grid.Append(new Vertex(new Vector2f(-MapSizeX, y + thickness), color));
            }
        }
        private void UpdateCells(RenderWindow window)
        {
            foreach (var cell in Objects.GetMoveblaObjects())
            {
                cell.Update(window);
            }
            UpdateCellsMass();
        }
        private void UpdateCellsMass()
        {
            bool upd = false;
            foreach (var obj in Objects.GetDrawableObjects())
            {
                if (obj is not PlayerCell cell) continue;
                if (cell.Mass >= 200 && Timer.GameTime - _lastMassUpdate > 0.2)
                {
                    cell.Mass *= 0.999f;
                    upd = true;
                }
            }
            if(upd == true) _lastMassUpdate = Timer.GameTime;

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
            foreach (var cellManagers in Objects.GetCellsManagers().OfType<ICellManager<Cell>>())
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
                        if (Logic.CanEat(cell, virus))
                        {
                            cell.Mass += virus.Mass;
                            cells.Add(cell);
                            Objects.Remove(virus);
                        }
                    }

                }

                foreach (var cell in cells)
                {
                    splittableCell.VirusSplit(cell);
                }
            }
        }
        private void CheckFoodEating(RenderWindow window)
        {
            var foods = Objects.GetFoodTree();

            foreach (var cell in Objects.GetCellsManagers().OfType<ICellManager<Cell>>())
            {
                foreach (var cl in cell.Cells)
                {
                    float searchRange = 50f + cl.Radius;
                    Envelope searchEnv = new Envelope(
                        cl.X - searchRange, cl.X + searchRange,
                        cl.Y - searchRange, cl.Y + searchRange);

                    var nearbyFoods = foods.Query(searchEnv);

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
            var nearCell = Objects.GetCellsTree();
            for (int i = 0; i < allCells.Count; i++)
            {
                float searchRange = 50f + allCells[i].Radius;
                var searchEnv = new Envelope(
                    allCells[i].X - searchRange, allCells[i].X + searchRange,
                    allCells[i].Y - searchRange, allCells[i].Y + searchRange);
                var nearbyCells = nearCell.Query(searchEnv);
                for (int j = 0; j < nearbyCells.Count; j++)
                {
                    var eater = allCells[i];
                    var victim = nearbyCells[j];
                    if (eater is IMergeable cell1 && victim is IMergeable cell2)
                    {
                        if (cell1.ID == cell2.ID) continue;
                    }
                    if (Logic.CanEat(eater, victim))
                    {
                        eater.Mass += victim.Mass;
                        Objects.RemoveCellFromAllManagers(victim);
                    }
                    else if (Logic.CanEat(victim, eater))
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
            float dist = Logic.GetDistanceBetweenCells(cell, food);
            if (dist < cell.Radius + food.Radius * 4 && food.IsEaten == false)
            {
                food.Target = cell;
                food.IsEaten = true;
            }
            if (dist < cell.Radius)
            {
                float prevRadius = cell.Radius;
                cell.Mass += food.Mass;
                food.ChangePos(MapSizeX, MapSizeY);
                food.IsEaten = false;
            }
        }
        private bool IsInViewZone(IDrawable obj, View camera)
        {
            return Math.Abs(camera.Center.X - obj.X) < camera.Size.X / 2 + obj.Radius * 4 &&
                   Math.Abs(camera.Center.Y - obj.Y) < camera.Size.Y / 2 + obj.Radius * 4;
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
            if (_player == null || _player.Cells.Count == 0)
                return _camera.Center;
            Vector2f vec = new();
            foreach (var cell in _player.Cells)
            {

                count++;
                center += new Vector2f(cell.X, cell.Y);

            }
            return center / count;
        }
        private void UpdateCameraSize(View camera, RenderWindow window)
        {
            if (_player == null || _player.Cells.Count == 0)
                return;
            float massDifference = (_player.GetTotalMass() - 200) / 50;
            float aspectRatio = window.Size.X / (float)window.Size.Y;
            _camera.Size = new Vector2f((400 + massDifference) * aspectRatio, 400 + massDifference);
        }

        public void Run()
        {
            var settings = new ContextSettings
            {
                AntialiasingLevel = 5

            };
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Agario", Styles.Default, settings);
            //window.SetVerticalSyncEnabled(true);
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
               
                Objects.UpdateObjects();
               
                window.SetView(_camera);
                window.DispatchEvents();
                window.Clear(Color.White);
                Draw(window);
                Update(window);
                _camera.Center = GetCenterCamera();
                UpdateCameraSize(_camera, window);
                window.Display();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
