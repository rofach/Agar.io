using SFML.System;
using System.Text.Json.Serialization;


namespace Agario.GameLogic
{
    public static class Timer
    {
        private static Clock s_delta = new Clock();
        private static float _totalActiveGameTime = 0f;
        public static float DeltaTime { get; private set; }
        public static float GameTime { get { return _totalActiveGameTime; } }
        public static void Update()
        {
            DeltaTime = s_delta.Restart().AsSeconds();
            _totalActiveGameTime += DeltaTime;
        }
                
        public static void ResetDeltaClock()
        {
            s_delta.Restart();
            DeltaTime = 0.001f;
        }
        public static void SetTotalGameTime(float loadedTime)
        {
            _totalActiveGameTime = loadedTime;
        }
    }
}
