using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using System.Collections.Generic;

namespace DummyPlayerBot.AI.Heuristics
{
    public class HealingHeuristic : IHeuristic
    {
        public int MaxHp { get; }
        public bool Nearest { get; }

        public HealingHeuristic(int maxHp, bool nearest = true)
        {
            MaxHp = maxHp;
            Nearest = nearest;
        }

        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            if (level.Player.Health < MaxHp && level.HealthPacks.Any())
            {
                enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(enviroment.TravelMap, enviroment.EnemyMap);

                foreach (var hp in Nearest ? level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).Select(h => h.Location) : BestHP(level, enviroment))
                {
                    var path = map.FindPath(level.Player.Location, hp);
                    if (path == null) //бывают ситуации, когда полуживой монстр закрывает на проход к аптечкам. Добиваем его (раз нет других вариантов)
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

        public IEnumerable<Location> BestHP(LevelView level, Enviroment enviroment)
        {
            var c = new Location(0, 0);
            var neirhood = new[] { c.Up(),  c.Down(),  c.Left(),  c.Right(), c.Up().Up(), /*c.Down().Down(), c.Left().Left(), c.Right().Right()*/ };
            return level.HealthPacks
                .OrderByDescending(h => neirhood
                    .Count(p => enviroment.TravelMap.IsTravaible(p + new Offset(h.Location.X, h.Location.Y))))
                .ThenBy(h => h.Location.Distance(level.Player.Location))
                .Select(h => h.Location);
        }
    }
}