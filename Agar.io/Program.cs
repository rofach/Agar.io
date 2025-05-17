
namespace Agario
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(5000, 30, 100);
            game.Run();
        }
    }
}
