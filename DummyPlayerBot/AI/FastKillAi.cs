using System;
using System.Collections.Generic;
using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public class FastKillAi : IAi
    {
        public int CriticalPercentageInactivity => 10;
        public Location Exit { get; }
        public WallMap WallMap;
        public ItemView BestItem(LevelView level) => level.Items.OrderByDescending(i => Math.Max(i.AttackBonus, i.DefenceBonus)).ThenByDescending(i => i.AttackBonus + i.DefenceBonus).First();

        public FastKillAi(LevelView level)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            WallMap = new WallMap(level, 2);
        }

        public Turn Iteration(LevelView level, IMessageReporter messageReporter, out bool isAttack)
        {
            isAttack = false;
            var monsterMap = new EnemyMap(level, 1);
            var trapMap = new TrapMap(level);
            var travelMap = Map.Sum(trapMap, WallMap);
            var pathMap = Map.Sum(monsterMap, travelMap);
            List<Location> path = null;
            if ((level.Player.Health < 55 || level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1) && level.HealthPacks.Any())
            {
                path = pathMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                messageReporter.ReportMessage("Healing");
            }
            else if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                messageReporter.ReportMessage("Attack");
                isAttack = false;
                return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
            }
            else if (level.Monsters.Any())
            {
                int i = 0;
                path = travelMap.FindPath(level.Player.Location, level
                    .Monsters.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                if (i > 10)
                return Turn.None;           
                messageReporter.ReportMessage("Far attack");
            }
            else if (level.Player.Health < 100 && level.HealthPacks.Any())
            {
                messageReporter.ReportMessage("100 Healing");
                path = pathMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
            }
            else if (Math.Max(BestItem(level).AttackBonus, BestItem(level).DefenceBonus) > Math.Max(level.Player.TotalAttack - level.Player.Attack, level.Player.TotalDefence - level.Player.Defence))
            {
                path = pathMap.FindPath(level.Player.Location, BestItem(level).Location);
            }
            else
            {
                messageReporter.ReportMessage("Leave");
                var leaveMap  = Map.Sum(travelMap, new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1));
                path = leaveMap.FindPath(level.Player.Location, Exit);
            }
            if (path != null)
                return Turn.Step(path[1] - path[0]);
            return Turn.None;
        }

        public Turn HandleCycle(LevelView level)
        {
            if (level.HealthPacks.Any())
            {
                var env = new Enviroment(level);
                env.Update(level);
                foreach (var hp in level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)))
                {
                    var path = env.TravelMap.FindPath(level.Player.Location, hp.Location);
                    if (path != null && path.Count > 1)
                        return Turn.Step(path[1] - path[0]);

                }
            }
            return null;
        }
    }
}