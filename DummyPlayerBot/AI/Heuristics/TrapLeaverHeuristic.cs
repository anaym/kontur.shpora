using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public class TrapLeaverHeuristic : IHeuristic
    {
        public Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            if (level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1 && level.Monsters.Count() > 2)
            {
                enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(enviroment.TravelMap, enviroment.EnemyMap);
                enviroment.EnemyMap.Multiplyer = 2;
                var spot = new[] { enviroment.Exit, enviroment.Start }.OrderByDescending(s => s.Distance(level.Player.Location)).First();
                var target = spot + new Offset(1, 0);
                var near =
                    level.Player.Location.Near()
                        .Where(p => map.IsTravaible(p) && enviroment.WallMap.GetWeight(p) < 2)
                        .OrderBy(p => level.Monsters.Count(m => m.Location.IsInRange(p, 1)));

                foreach (var location in near)//new
                {
                    if (level.Monsters.Count(m => m.Location.IsInRange(location, 1)) > 0)
                        break;
                    var p = map.FindPath(level.Player.Location, target);
                    if (p != null && p.Count > 1)
                    {
                        return Turn.Step(p[1] - p[0]);
                    }
                }

                var path = map.FindPath(level.Player.Location, target);
                if (path == null)
                    if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
                    {
                        isAttack = true;
                        return
                            Turn.Attack(
                                level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location -
                                level.Player.Location);
                    }
                    else
                        return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            return null;
        }
    }
}