using System.Linq;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class AttackTask : ITask
    {
        public Location Target { get; }

        public AttackTask(Location target)
        {
            Target = target;
        }

        public string Name => "Attack";

        public Turn Step(LevelView level, Enviroment enviroment)
        {
            return Turn.Attack(Target - level.Player.Location);
        }

        public bool IsFinished(LevelView level, Enviroment enviroment)
        {
            return !level.Monsters.Any(m => m.Location.Equals(Target));
        }
    }
}