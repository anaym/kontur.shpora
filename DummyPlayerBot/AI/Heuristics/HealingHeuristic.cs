using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class HealingHeuristic : IHeuristic
    {
        public int MaxHp { get; }

        public HealingHeuristic(int maxHp)
        {
            MaxHp = maxHp;
        }

        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Player.Health < MaxHp && level.HealthPacks.Any())
            {
                enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(enviroment.TravelMap, enviroment.EnemyMap);
                enviroment.EnemyMap.Multiplyer = 2;

                foreach (var hp in level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)))
                {
                    var path = map.FindPath(level.Player.Location, hp.Location);
                    if (path == null)
                    {
                        if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
                        {
                            isAttack = true;
                            return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
                        }

                    }
                    else
                    {
                        return Turn.Step(path[1] - path[0]);
                    }
                }
            }
            return null;
        }
    }
}