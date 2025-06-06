﻿using Agario.Cells.Bots;
using Agario.GameLogic;

namespace Agario.Strategies
{
    public class BotBehaviorsManager
    {
        private IBehavior? _aggressiveStrategy;
        private IBehavior? _safeStrategy;
        
        public BotBehaviorsManager() { 
            _aggressiveStrategy = new AggressiveBehavior();
            _safeStrategy = new SafeBehavior();
        }
        
        public void UpdateBehavior()
        {
            foreach (var manager in Objects.GetCellsManagers().Where(m => m is Bot))
            {
                var bot = manager as Bot;
                if (bot.Cells.Count > 1)
                    bot.Behavior = _safeStrategy;
                else
                    bot.Behavior = _aggressiveStrategy;
            }
        }
    }
}
