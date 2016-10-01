using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class BonusCollectorHeuristic : IHeuristic
    {
        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            var bonusCollectorMap = Map.Sum(enviroment.WallMap, enviroment.EnemyMap, enviroment.TrapMap);
            if (level.Items.Any(i => i.IsBetter(level.Player)))
            {
                var path = bonusCollectorMap.FindPath(level.Player.Location, level.Items.First(i => i.IsBetter(level.Player)).Location);
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            return null;
        }
    }
}