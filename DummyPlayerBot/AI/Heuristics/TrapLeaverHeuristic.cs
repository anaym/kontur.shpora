﻿using System.Linq;
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
            //если монстры атакуют более чем двумя - убегаем на другой конец карты
            if ((level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1 || level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 3)) > 4) && level.Monsters.Count() > 2)
            {
                enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(enviroment.TravelMap, enviroment.EnemyMap);
                var b = enviroment.Start;
                var e = enviroment.Exit;
                var spot = new[] { b, e, new Location(b.X, e.Y), new Location(e.X, b.Y) }
                    .OrderBy(t => level.Monsters.Count(m => m.Location.IsInRange(t, 4)))  
                    //.ThenByDescending(s => s.Distance(level.Player.Location))
                    .First(s => s.Near().Any(p => enviroment.TravelMap.IsTravaible(p)));
                var target = spot.Near().Where(p => enviroment.TravelMap.IsTravaible(p)).OrderBy(p => p.Distance(level.Player.Location)).First(); //эта клетка вроде всегда свободна
                var bwm = new WallMap(level, 7);
                var near =
                    level.Player.Location.Near()
                        .Where(p => map.IsTravaible(p) && bwm.GetWeight(p) < 2)
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
                if (path == null || path.Count <= 1)
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