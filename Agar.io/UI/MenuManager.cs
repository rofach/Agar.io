using Agario.GameLogic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Agario.UI
{
    public enum MenuState
    {
        None,
        Initial,
        Paused
    }

    public class MenuManager
    {
        private RenderWindow _window;
        private Font _font;
        private Game _gameInstance;

        public MenuState CurrentMenuState { get; private set; } = MenuState.Initial;
        private List<Button> _currentButtons = new List<Button>();

        private Button _playButton;
        private Button _loadButtonInitial;
        private Button _continueButton;
        private Button _loadButtonPaused;
        private Button _saveButton;

        private Vector2f _buttonFixedSize = new Vector2f(200f, 50f);
        private uint _characterFixedSize = 20;
        private float _verticalFixedSpacing = 25f;

        public bool IsVisible => CurrentMenuState != MenuState.None;

        public MenuManager(RenderWindow window, Font font, Game gameInstance)
        {
            _window = window;
            _font = font;
            _gameInstance = gameInstance;

            InitializeButtons();
            SetMenuState(MenuState.Initial);
        }

        private void InitializeButtons()
        {
            Color buttonColor = new Color(255, 100, 0, 220);
            Color textColor = Color.White;
            _playButton = new Button("Play", new Vector2f(), _buttonFixedSize, buttonColor, textColor, _font, _characterFixedSize, "Play");
            _loadButtonInitial = new Button("Load Game", new Vector2f(), _buttonFixedSize, buttonColor, textColor, _font, _characterFixedSize, "Load");
            _continueButton = new Button("Continue", new Vector2f(), _buttonFixedSize, buttonColor, textColor, _font, _characterFixedSize, "Continue");
            _saveButton = new Button("Save Game", new Vector2f(), _buttonFixedSize, buttonColor, textColor, _font, _characterFixedSize, "Save");
            _loadButtonPaused = new Button("Load Game", new Vector2f(), _buttonFixedSize, buttonColor, textColor, _font, _characterFixedSize, "Load");
        }

        public void SetMenuState(MenuState newState)
        {
            CurrentMenuState = newState;
            _currentButtons.Clear();

            if (CurrentMenuState == MenuState.Initial)
            {
                _currentButtons.Add(_playButton);
                _currentButtons.Add(_loadButtonInitial);
            }
            else if (CurrentMenuState == MenuState.Paused)
            {
                _currentButtons.Add(_continueButton);
                _currentButtons.Add(_saveButton);
                _currentButtons.Add(_loadButtonPaused);
            }
            UpdateUILayout(); 
        }

        public void HandleResize()
        {
            UpdateUILayout();
        }

        public void UpdateUILayout()
        {
            View preiousView = _window.GetView();
            var newView = new View(new Vector2f(_window.Size.X / 2, _window.Size.Y / 2), new Vector2f(_window.Size.X, _window.Size.Y));
            _window.SetView(newView);
            if (_window == null) return;
            float centerX = _window.Size.X / 2f;
            
            if (CurrentMenuState == MenuState.Initial)
            {
                float groupHeight = 2 * _buttonFixedSize.Y;
                float startY = (_window.Size.Y - groupHeight) / 2f;
                _playButton?.UpdatePosition(new Vector2f(centerX - _buttonFixedSize.X / 2f, startY));
                _loadButtonInitial?.UpdatePosition(new Vector2f(centerX - _buttonFixedSize.X / 2f, startY + _buttonFixedSize.Y + _verticalFixedSpacing));
            }
            else if (CurrentMenuState == MenuState.Paused)
            {
                float groupHeight = 3 * _buttonFixedSize.Y + 2 * _verticalFixedSpacing;
                float startY = (_window.GetView().Center.Y - groupHeight) / 2f;

                _continueButton?.UpdatePosition(new Vector2f(centerX - _buttonFixedSize.X / 2f, startY));
                _saveButton?.UpdatePosition(new Vector2f(centerX - _buttonFixedSize.X / 2f, startY + _buttonFixedSize.Y + _verticalFixedSpacing));
                _loadButtonPaused?.UpdatePosition(new Vector2f(centerX - _buttonFixedSize.X / 2f, startY + 2 * (_buttonFixedSize.Y + _verticalFixedSpacing)));
            }
            _window.SetView(preiousView);
        }

        public void HandleClick(Vector2i mousePosition)
        {
            if (!IsVisible) return;
            foreach (var button in _currentButtons)
            {
                if (button.IsMouseOver(mousePosition))
                {
                    ProcessButtonAction(button.Action);
                    return;
                }
            }
        }

        private void ProcessButtonAction(string action)
        {
            switch (action)
            {
                case "Play":
                    _gameInstance.StartNewGame();
                    break;
                case "Continue":
                    _gameInstance.ResumeGame();
                    break;
                case "Load":
                    _gameInstance.LoadGame();
                    break;
                case "Save":
                    _gameInstance.SaveGame();
                    break;
            }
        }

        public void Draw(RenderWindow window)
        {
            if (!IsVisible) return;

            View preiousView = window.GetView();
            var newView = new View(new Vector2f(_window.Size.X / 2, _window.Size.Y / 2), new Vector2f(_window.Size.X, _window.Size.Y));
            window.SetView(newView); 

            if (CurrentMenuState == MenuState.Paused)
            {
                RectangleShape dimOverlay = new RectangleShape(new Vector2f(_window.Size.X, _window.Size.Y));
                dimOverlay.Position = window.GetView().Center - new Vector2f(_window.Size.X / 2, _window.Size.Y / 2);
                dimOverlay.FillColor = new Color(0, 0, 0, 100);
                window.Draw(dimOverlay);
               
            }

            foreach (var button in _currentButtons)
            {
                button.Draw(window);
            }

            //window.SetView(preiousView);
        }
    }
   
    
}