using System.Linq;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class FarAttackHeuristic : IHeuristic
    {
        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Monsters.Any())
            {
                var map = enviroment.TravelMap;
                var path = enviroment.AttackMap.FindPath(level.Player.Location, level.Monsters.OrderBy(h => map.GetDistance(level.Player.Location, h.Location)).First().Location);
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            return null;
        }
    }
}