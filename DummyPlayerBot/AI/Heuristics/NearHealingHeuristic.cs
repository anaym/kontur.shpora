using System.Linq;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class NearHealingHeuristic : IHeuristic 
    {
        public NearHealingHeuristic(int hpLim)
        {
            this.HpLim = hpLim;
        }

        public int HpLim { get; }

        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Player.Health < HpLim && level.HealthPacks.Any(p => p.Location.Distance(level.Player.Location) == 1))
            {
                return Turn.Step(level.HealthPacks.First(p => p.Location.Distance(level.Player.Location) == 1).Location - level.Player.Location);
            }
            return null;
        }
    }
}