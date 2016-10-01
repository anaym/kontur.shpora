using System.Linq;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class NearHealingHeuristic : IHeuristic
    {
        public int HpMax { get; }

        public NearHealingHeuristic(int hpMax)
        {
            HpMax = hpMax;
        }

        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Player.Health >= HpMax)
                return null;
            if (level.HealthPacks.Any(h => h.Location.Distance(level.Player.Location) < 1))
            {
                return Turn.Step(level.HealthPacks.First(h => h.Location.Distance(level.Player.Location) < 1).Location - level.Player.Location);
            }
            return null;
        }
    }
}