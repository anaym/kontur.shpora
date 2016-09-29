using System;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class TravelTaskGenerator : ITaskGenerator
    {
        private readonly Func<LevelView, Location> targetExtractor;
        private Func<LevelView, bool> canReplace;
        private string name;

        public TravelTaskGenerator(Func<LevelView, Location> targetExtractor, String name, Func<LevelView, bool> canReplace = null)
        {
            this.targetExtractor = targetExtractor;
            this.canReplace = canReplace;
            this.name = name;
        }


        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {
            return (canReplace?.Invoke(level) ?? true) &&  Map.Sum(enviroment.EnemyMap, enviroment.WallMap, enviroment.TrapMap).IsTravaible(targetExtractor(level));
        }

        public ITask Generate(LevelView level, Enviroment enviroment)
        {
            return new TravelTask(targetExtractor(level), name);
        }
    }
}