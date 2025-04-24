using SFML.System;


namespace Agario
{
    public static class Timer
    {
        static private Clock delta = new Clock();
        static private Clock timer = new Clock();
        public static float DeltaTime { get; private set; }
        public static float GameTime { get { return timer.ElapsedTime.AsSeconds(); } }
        public static void Update()
        {
            DeltaTime = delta.Restart().AsSeconds();
        }

        
    }
}
