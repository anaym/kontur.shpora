using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class NearAttackGenerator : ITaskGenerator
    {
        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {
            return level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1));
        }

        public ITask Generate(LevelView level, Enviroment enviroment)
        {
            return new AttackTask(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location);
        }
    }
}