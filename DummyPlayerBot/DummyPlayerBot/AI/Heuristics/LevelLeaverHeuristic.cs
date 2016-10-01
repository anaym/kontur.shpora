using System.Linq;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class LevelLeaverHeuristic : IHeuristic
    {
        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            var bonusCollectorMap = Map.Sum(enviroment.WallMap, enviroment.EnemyMap, enviroment.TrapMap);
            if (!level.Monsters.Any())
            {
                var path = enviroment.TravelMap.FindPath(level.Player.Location, enviroment.Exit);
                if (path == null)
                {
                    path = bonusCollectorMap.FindPath(level.Player.Location, enviroment.Exit);
                }
                if (path == null)
                {
                    return Turn.None;
                }
                return Turn.Step(path[1] - path[0]);
            }
            return null;
        }
    }
}