using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class FarAttackGenerator : ITaskGenerator
    {
        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {
            return (task?.IsFinished(level, enviroment) ?? true) && level.Monsters.Any();
        }

        public ITask Generate(LevelView level, Enviroment enviroment)
        {
            return new FarAttack(level.Monsters.OrderBy(m => m.Location.Distance(level.Player.Location)).First().Name);
        }
    }
}