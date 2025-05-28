using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agario.UI
{
    public class Button
    {
        private Font _font;
        private uint _characterSize;
        private Vector2f _size;
        private RenderWindow _window;
        private Color _originalFillColor;
        public RectangleShape Shape { get; private set; }
        public Text ButtonText { get; private set; }
        public string Action { get; private set; }

        public Button(string text, Vector2f position, Vector2f size, Color fillColor, Color textColor, Font font, uint characterSize, string action)
        {
            Action = action;
            _font = font;
            _characterSize = characterSize;
            _size = size;
            Shape = new RectangleShape(_size)
            {
                FillColor = fillColor
            };

            ButtonText = new Text(text, _font, _characterSize)
            {
                FillColor = textColor
            };
            _originalFillColor = fillColor;
            UpdatePosition(position);
        }

        public void Resize(Vector2f newShapeSize, uint newCharacterSize)
        {
            Shape.Size = newShapeSize;
            ButtonText.CharacterSize = newCharacterSize;
        }

        public void UpdatePosition(Vector2f newTopLeftPosition)
        {
            Shape.Position = newTopLeftPosition;
            FloatRect textBounds = ButtonText.GetLocalBounds();
            
            ButtonText.Origin = new Vector2f(textBounds.Left + textBounds.Width / 2f,
                                             textBounds.Top + textBounds.Height / 2f);
            ButtonText.Position = new Vector2f(Shape.Position.X + Shape.Size.X / 2f,
                                               Shape.Position.Y + Shape.Size.Y / 2f);
        }
        public void Hover()
        {
            Shape.FillColor = new Color(255, 150, 0, 220);
        }
        public void ResetColor()
        {
            Shape.FillColor = _originalFillColor; // Reset to original color
        }
        public void Draw(RenderWindow window)
        {
            window.Draw(Shape);
            window.Draw(ButtonText);
        }

        public bool IsMouseOver(Vector2i mousePosition)
        {
            return Shape.GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y);
        }
    }
}
