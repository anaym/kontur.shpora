using System.Linq;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class NearAttackHeurisitc : IHeuristic
    {
        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                var monster = level.Monsters.Where(m => m.Location.IsInRange(level.Player.Location, 1)).OrderBy(m => m.Health).First();
                isAttack = true;
                return Turn.Attack(monster.Location - level.Player.Location);
            }
            return null;
        }
    }
}