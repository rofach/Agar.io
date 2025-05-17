using Agario.Cells.Bots;
using SFML.System;


namespace Agario.Strategies
{
    public interface IBehavior
    {
        Vector2f FindNewTargetPoint(Bot bot);
    }
}
