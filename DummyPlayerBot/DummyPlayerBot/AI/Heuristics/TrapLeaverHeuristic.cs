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
            //если монстры атакуют более чем двумя или их рядом слишком много - убегаем на другой конец карты
            if ((level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1
                || level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 3)) > 3) && level.Monsters.Count() > 2)
            {
                enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(enviroment.TravelMap, enviroment.EnemyMap);

                var spots = GetSaveSpots(level, enviroment);

                Location target = new Location(0, 0);
                foreach (var sp in spots)
                {
                    target = sp.Near().Where(p => enviroment.TravelMap.IsTravaible(p)).OrderBy(p => p.Distance(level.Player.Location)).FirstOr(new Location(-1, -1)); //эта клетка вроде всегда свободна
                    if (target.X >= 0)
                        break;
                }

                var solve = ManualStep(map, target, level, enviroment, out isAttack);
                if (solve != null)
                    return solve;
                return AutomaticStep(map, target, level, out isAttack);
            }
            return null;
        }

        public IOrderedEnumerable<Location> GetSaveSpots(LevelView level, Enviroment enviroment)
        {
            var b = enviroment.Start;
            var e = enviroment.Exit;
            return new[] { b, e, new Location(b.X, e.Y), new Location(e.X, b.Y) }
                .OrderBy(s => level.Monsters.Count(m => s.IsInRange(m.Location, 3)))
                .ThenByDescending(s => s.Distance(level.Player.Location));
        }

        //пробуем пойти в клетку, которую атакуют меньше монстров (если рядом нет стен)
        public Turn ManualStep(Map map, Location target, LevelView level, Enviroment enviroment, out bool isAttack)
        {
            isAttack = false;
            var near =
                    level.Player.Location.Near()
                        .Where(p => map.IsTravaible(p) && enviroment.WallMap.GetWeight(p) < 2)
                        .OrderBy(p => level.Monsters.Count(m => m.Location.IsInRange(p, 1)));

            foreach (var location in near)
            {
                if (level.Monsters.Count(m => m.Location.IsInRange(location, 1)) > 0)
                    continue;
                var p = map.FindPath(level.Player.Location, target);
                if (p != null && p.Count > 1)
                {
                    return Turn.Step(p[1] - p[0]);
                }
            }
            return null;
        }

        //идем туда, куда будет проложен маршрут по карте
        public Turn AutomaticStep(Map map, Location target, LevelView level, out bool isAttack)
        {
            isAttack = false;
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
    }
}