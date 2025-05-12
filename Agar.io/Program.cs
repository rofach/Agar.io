
namespace Agario
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(3000, 60);
            game.Run();
        }
    }
}
