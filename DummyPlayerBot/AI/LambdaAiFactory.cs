using System;
using DummyPlayerBot.AI;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class LambdaAiFactory : IAiFactory
    {

        public LambdaAiFactory(Func<LevelView, int, IAi> botCreator)
        {
            this.BotCreator = botCreator;
        }

        private Func<LevelView, int, IAi> BotCreator { get; }

        public IAi CreateBot(LevelView level, int levelNumber) => BotCreator(level, levelNumber);
    }
}