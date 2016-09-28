using System;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class LambdaBotFactory : IBotFactory
    {

        public LambdaBotFactory(Func<LevelView, int, IBot> botCreator)
        {
            this.BotCreator = botCreator;
        }

        private Func<LevelView, int, IBot> BotCreator { get; }

        public IBot CreateBot(LevelView level, int levelNumber) => BotCreator(level, levelNumber);
    }
}