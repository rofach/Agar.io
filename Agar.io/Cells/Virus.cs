using SFML.Graphics;
using SFML.System;
using Agario.GameLogic;
using Color = SFML.Graphics.Color;

namespace Agario.Cells
{
    public class Virus : Cell
    {
        private float _spikeLengthFactor;
        private int _spikeCount;
        private VertexArray _virus;
        private Color _color;
        public Virus(int initialMass = 500, float spikeLengthFactor = 0.1f, int spikeCount = 32)
        {
            _spikeLengthFactor = spikeLengthFactor;
            _spikeCount = spikeCount;

            var rnd = new Random();
            Position = new Vector2f(
                rnd.Next(-Game.MapSizeX, Game.MapSizeX),
                rnd.Next(-Game.MapSizeY, Game.MapSizeY)
            );
            Mass = initialMass;
            _color = new Color(0, 255, 0);
            CreateVirusShape();
        }
        private void CreateVirusShape()
        {
            float baseRadius = Radius - OutLineThickness;
            float spikeLength = baseRadius * _spikeLengthFactor;
            int pointCount = _spikeCount * 2;

            _virus = new VertexArray(PrimitiveType.TriangleFan, (uint)(pointCount + 2));
            _virus[0] = new Vertex(new Vector2f(0, 0), _color);

            for (int i = 0; i <= pointCount; i++)
            {
                float angle = i * (360f / pointCount) * (MathF.PI / 180f);
                float curR = (i % 2 == 0) ? baseRadius + spikeLength : baseRadius;
                var pos = new Vector2f(MathF.Cos(angle) * curR, MathF.Sin(angle) * curR);
                _virus[(uint)(i + 1)] = new Vertex(pos, _color);
            }
        }
        public override void Draw(RenderWindow window)
        {
            var states = RenderStates.Default;
            states.Transform.Translate(Position);

            window.Draw(_virus, states);
        }
       
    }
}



