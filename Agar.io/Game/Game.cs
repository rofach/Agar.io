using Agario.Cells;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using Agario.Strategies;
using Agario.Cells.Bots;
using Agario.GameLogic;
using Agario.UI;
using System.Reflection;
using Newtonsoft.Json;
namespace Agario.GameLogic
{
    sealed public class Game
    {
        public const int MapSizeX = 5000, MapSizeY = 5000;
        public GameState CurrentGameState { get; private set; } = GameState.MainMenu;
        public static readonly Font GameFont = new Font("Moodcake.ttf");
        
        private float _lastMassUpdate;
        private View? _camera;
        private Player? _player;
        private BotBehaviorsManager? _botStrategiesManager;
        private VertexArray? _grid;
       
        private MenuManager _menuManager;
        private static int _nextId = 0;

        private int _initialFoodCount = 200;
        private int _initialBotsCount = 10;
        private int _initialVirusCount = 5;

        public Game(int foodCount = 200, int botsCount = 10, int virusCount = 5)
        {
            _initialFoodCount = foodCount;
            _initialBotsCount = botsCount;
            _initialVirusCount = virusCount;
        }

        private void InitializeEssentialComponents(RenderWindow window)
        {
            _menuManager = new MenuManager(window, GameFont, this);
            _camera = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            CreateGrid();
        }
        public void StartNewGame()
        {
            Objects.ClearAll();
            for (int i = 0; i < _initialFoodCount; i++)
            {
                Objects.Add(new Food(50));
            }
            _player = new Player(GetNextId());
            Objects.Add(_player);
            for (int i = 0; i < _initialBotsCount; i++)
            {
                if (i % 3 == 0) Objects.Add(new TeleportBot(GetNextId()));
                else if (i % 3 == 1) Objects.Add(new SpeedBot(GetNextId()));
                else Objects.Add(new MassBot(GetNextId()));
            }
            for (int i = 0; i < _initialVirusCount; i++)
            {
                Objects.Add(new Virus(1000));
            }

            _botStrategiesManager = new BotBehaviorsManager();

            if (_camera != null && _player != null && _player.Cells.Count > 0)
            {
                _camera.Center = GetCenterCamera();

            }
            else if (_camera != null)
            {
                _camera.Center = new Vector2f(MapSizeX / 2f, MapSizeY / 2f);
            }

            CurrentGameState = GameState.Playing;
            Timer.SetTotalGameTime(0f);
            Timer.ResetDeltaClock();
        }
        private int GetNextId()
        {
            return _nextId++;
        }
        public void ResumeGame()
        {
            if (CurrentGameState == GameState.Paused)
            {
                CurrentGameState = GameState.Playing;
                Timer.ResetDeltaClock();
            }
        }
        public void PauseGame(RenderWindow window)
        {

            if (CurrentGameState == GameState.Playing)
            {
                CurrentGameState = GameState.Paused;
                _menuManager.SetMenuState(MenuState.Paused);
                UpdateCameraSize(_camera, window);
            }
        }
        public void ShowInitialMenu()
        {
            CurrentGameState = GameState.MainMenu;
            _menuManager.SetMenuState(MenuState.Initial);
            _player = null;
        }

        private void Draw(RenderWindow window)
        {
            window.SetView(_camera);
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
            Objects.UpdateObjects();
            RecreateBot();
            if (CurrentGameState == GameState.Playing && (_player == null || _player.Cells.Count == 0))
            {
                ShowInitialMenu();
            }
        }
        private void DrawObjects(RenderWindow window)
        {
            List<Cell> cellsToDraw = new List<Cell>();
            foreach (var obj in Objects.GetDrawableObjects())
            {
                if (obj is Cell cell)
                {
                    if (IsInViewZone(cell, _camera))
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
                    cell.Mass *= 0.998f;
                    upd = true;
                }
            }
            if (upd == true) _lastMassUpdate = Timer.GameTime;

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
        private void RecreateBot()
        {
            var removedManagers = Objects.GetRemovedManagers();
            foreach (Bot bot in removedManagers.OfType<Bot>())
            {
                Objects.Add(bot);
                bot.Recreate();
            }
            Objects.ClearRemovedManagers();
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
        private bool IsInViewZone(Cell obj, View camera)
        {
            return Math.Abs(camera.Center.X - obj.X) < camera.Size.X / 2 + obj.Radius * 4 &&
                   Math.Abs(camera.Center.Y - obj.Y) < camera.Size.Y / 2 + obj.Radius * 4;
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

            float cellsSizesToAdd = 0;
            for (int i = 1; i < _player.Cells.Count; i++)
            {
                cellsSizesToAdd += Math.Min(_player.Cells[i].Radius / 2, 200); 
            }
            float sizeToAdd = MathF.Sqrt(3*_player.GetTotalMass()) + cellsSizesToAdd;
           
            float aspectRatio = window.Size.X / (float)window.Size.Y;
            _camera.Size = new Vector2f((400 + sizeToAdd) * aspectRatio, 400 + sizeToAdd);
        }

        public void Run()
        {
            var settings = new ContextSettings { AntialiasingLevel = 5 }; 
            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Agario", Styles.Default, settings); 

            InitializeEssentialComponents(window);
            window.Closed += OnCloseWindow;
            window.MouseButtonPressed += OnMouseButtonPressed;
            window.KeyPressed += OnKeyPressed;
            window.Resized += OnWindowResized;
            window.MouseMoved += OnMouseMoved;
            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear(Color.White);

                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        var view = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
                        window.SetView(view);
                        _menuManager.Draw(window);
                        Timer.ResetDeltaClock();
                        break;

                    case GameState.Playing:

                        Draw(window);
                        Update(window);
                        if (_camera != null && _player != null && _player.Cells.Count > 0)
                        {
                            _camera.Center = GetCenterCamera();
                            UpdateCameraSize(_camera, window);
                        }
                        
                        break;

                    case GameState.Paused:
                        Draw(window);
                        _menuManager.Draw(window);
                        break;
                }
                window.Display();
            }
        }
        private void OnCloseWindow(object? sender, EventArgs e)
        {
            if (sender is RenderWindow window)
            {
                window.Close();
            }
        }
        private void OnWindowResized(object sender, SizeEventArgs e)
        {
            if (_camera != null)
            {
                _camera.Size = new Vector2f(e.Width, e.Height);
            }
            if (_menuManager != null && _menuManager.IsVisible)
            {
                _menuManager.HandleResize();
            }
            UpdateCameraSize(_camera, (RenderWindow)sender);

        }
        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (CurrentGameState == GameState.MainMenu || CurrentGameState == GameState.Paused)
            {
                _menuManager.HandleClick(new Vector2i(e.X, e.Y));
            }
        }

        private void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                if (CurrentGameState == GameState.Playing)
                {
                    PauseGame(sender as RenderWindow);
                    UpdateCameraSize(_camera, (RenderWindow)sender);
                }
                else if (CurrentGameState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
            else if (e.Code == Keyboard.Key.Space)
                _player.SplitKeyPressed();

        }
        private void OnMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            if (CurrentGameState == GameState.MainMenu || CurrentGameState == GameState.Paused)
            {
                _menuManager.HandleMouseMove(new Vector2i(e.X, e.Y));
            }
        }

        public void SaveGame()
        {
            SerializableGameData gameData = new SerializableGameData
            {
                LastMassUpdate = _lastMassUpdate,
                PlayerInstance = _player,
                SavedTotalGameTime = Timer.GameTime
            };

            if (Objects.GetCellsManagers() != null)
            {
                foreach (var manager in Objects.GetCellsManagers())
                {
                    if (manager is Bot bot && manager != _player)
                    {
                        gameData.Bots.Add(bot);
                    }
                }
            }

            if (Objects.GetDrawableObjects() != null)
            {
                foreach (var drawable in Objects.GetDrawableObjects())
                {
                    if (drawable is Virus virus)
                    {
                        gameData.Viruses.Add(virus);
                    }
                    else if (drawable is Food food)
                    {
                        gameData.Foods.Add(food);
                    }
                }
            }

            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,

            };

            try
            {
                string json = JsonConvert.SerializeObject(gameData, settings);
                File.WriteAllText("game_save.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
            }
        }
        public void LoadGame()
        {
            Objects.ClearAll();
            try
            {
                string json = File.ReadAllText("game_save.json");
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                SerializableGameData? loadedData = JsonConvert.DeserializeObject<SerializableGameData>(json, settings);
                Timer.SetTotalGameTime(loadedData.SavedTotalGameTime);
                Timer.ResetDeltaClock();
                if (loadedData == null)
                {
                    StartNewGame();
                    return;
                }

                _lastMassUpdate = loadedData.LastMassUpdate;
                _player = loadedData.PlayerInstance;
                if (_player != null)
                {
                    Objects.Add(_player);
                }

                if (loadedData.Bots != null)
                {
                    foreach (var bot in loadedData.Bots)
                    {
                        if (bot != null) Objects.Add(bot);
                    }
                }
                if (loadedData.Viruses != null)
                {
                    foreach (var virus in loadedData.Viruses)
                    {
                        if (virus != null) Objects.Add(virus);
                    }
                }
                if (loadedData.Foods != null)
                {
                    foreach (var food in loadedData.Foods)
                    {
                        if (food != null) Objects.Add(food);
                    }
                }
                _botStrategiesManager = new BotBehaviorsManager();

                if (_camera != null)
                {
                    if (_player != null && _player.Cells.Any())
                    {
                        _camera.Center = GetCenterCamera();
                    }
                    else
                    {
                        _camera.Center = new Vector2f(MapSizeX / 2f, MapSizeY / 2f);
                    }
                }

                CurrentGameState = GameState.Playing;
                Objects.UpdateObjects();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}. Starting new game");
                StartNewGame();
            }
        }
    }
    public class SerializableGameData
    {
        public Player PlayerInstance { get; set; }
        public List<Bot> Bots { get; set; } = new List<Bot>();
        public List<Virus> Viruses { get; set; } = new List<Virus>();
        public List<Food> Foods { get; set; } = new List<Food>();
        public float LastMassUpdate { get; set; }
        public float SavedTotalGameTime { get; set; }
    }
}
