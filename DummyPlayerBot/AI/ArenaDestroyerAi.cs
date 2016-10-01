﻿using System.Diagnostics;
using System.Linq;
using DummyPlayerBot.AI.Heuristics;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public class ArenaDestroyerAi : IAi
    {
        public Stopwatch Time { get; }
        public int CriticalTime { get; set; }
        public int CriticalPercentageInactivity => 40;
        public bool CycleDetected { get; private set; }
        public int MonsterStartHp { get; }
        public bool TaskCompleted { get; private set; }
        public Location Task { get; }

        public ArenaDestroyerAi(LevelView level)
        {
            Enviroment = new Enviroment(level, 2);
            var enemy = level.Monsters.First();
            Task = enemy.Location.Near(2).Where(l => Enviroment.WallMap.GetDistance(level.Player.Location, l) != null).OrderBy(l => enemy.Location.Distance(l)).FirstOr(new Location(-1, -1));
            if (Task.X < 0)
                TaskCompleted = true;
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            CriticalTime = 100;
            Time = new Stopwatch();
            Time.Start();

            if (level.Monsters.Any())
                MonsterStartHp = level.Monsters.First().Health;
        }

        public Enviroment Enviroment { get; set; }

        public Location Exit { get; }
        public Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack)
        {
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap, bonusIgnore);
            var s = new BonusCollectorHeuristic().Solve(level, new Enviroment(level), out isAttack);
            if (s != null)
                return s;
            if (level.Monsters.Any())
            {
                var monster = level.Monsters.First();
                var enemyHp = monster.Health;
                var healingHpLevel = 50;
                if (enemyHp < MonsterStartHp * 0.6 && level.HealthPacks.Count() < 4) //если враг пытается отрегениться - забираем его аптечку))
                    healingHpLevel = 60;
                if (enemyHp < MonsterStartHp*0.7) //если враг пытается отрегениться - забираем его аптечку))
                    healingHpLevel = 60;
                if (level.Player.Health < healingHpLevel && level.HealthPacks.Any())
                {
                    var path = travelMap.FindPath(level.Player.Location,
                        level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                    isAttack = false;
                    if (path != null && path.Count > 1)
                        return Turn.Step(path[1] - path[0]);
                    return Turn.None;
                }
            }
            isAttack = false;
            if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                var monster = level.Monsters.Where(m => m.Location.IsInRange(level.Player.Location, 1)).OrderBy(m => m.Health).First();
                isAttack = true;
                TaskCompleted = true;
                return Turn.Attack(monster.Location - level.Player.Location);
            }
            if (level.Monsters.Any() && !TaskCompleted)
            {
                var path = Enviroment.AttackMap.FindPath(level.Player.Location, Task);
                if (path != null && path.Count >= 2)
                {
                    return Turn.Step(path[1] - path[0]);
                }
                TaskCompleted = true;

            }
            if (level.Monsters.Any())
            {
                var target = level.Monsters.First().Location;
                var targets = target
                    .Near(3)
                    .Where(
                        p =>
                            p.X >= 0 && p.Y >= 0 && p.X < Enviroment.TravelMap.Width &&
                            p.Y < Enviroment.TravelMap.Height)
                    .Where(p => Enviroment.TravelMap.IsTravaible(p))
                    .OrderBy(p => p.Distance(target));

                foreach (var location in targets)
                {
                    var path = attackMap.FindPath(level.Player.Location, location);
                    if (path != null && path.Count > 1)
                        return Turn.Step(path[1] - path[0]);
                }
            }
            if (!ExitIsClosed(level))
            {
                Enviroment = new Enviroment(level, 2);
                var path = travelMap.FindPath(level.Player.Location, Exit);
                isAttack = false;
                if (path == null || path.Count < 2)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            isAttack = false;
            return Turn.None;
        }

        public Turn HandleCycle(LevelView level)
        {
            if (!level.Monsters.Any())
                return null;
            CycleDetected = true;
            return null;
        }

        public bool ExitIsClosed(LevelView level)
        {
            var exit = level.Field.GetCellsOfType(CellType.Exit).First();
            var neir = new Location[] { exit.Up(), exit.Down(), exit.Left(), exit.Right() };
            return neir.All(p => level.Field[p] == CellType.Wall);
        }
    }
}
