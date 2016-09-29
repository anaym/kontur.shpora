using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DummyPlayer;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class FastKillBot : IBot
    {
        public Location Exit { get; }
        public WallMap WallMap;
        public ItemView BestItem(LevelView level) => level.Items.OrderByDescending(i => Math.Max(i.AttackBonus, i.DefenceBonus)).ThenByDescending(i => i.AttackBonus + i.DefenceBonus).First();

        public FastKillBot(LevelView level)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            WallMap = new WallMap(level, 2);
        }

        public Turn Iteration(LevelView level, IMessageReporter messageReporter)
        {
            var monsterMap = new EnemyMap(level, 1);
            var trapMap = new TrapMap(level);
            var travelMap = Map.Sum(trapMap, WallMap);
            var pathMap = Map.Sum(monsterMap, travelMap);
            List<Location> path = null;
            if (level.Player.Health < 50 && level.HealthPacks.Any())
            {
                path = pathMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                messageReporter.ReportMessage("Healing");
            }
            else if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                messageReporter.ReportMessage("Attack");
                return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
            }
            else if (level.Monsters.Any())
            {
                int i = 0;
                path = travelMap.FindPath(level.Player.Location, level.Monsters.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
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
            //Thread.Sleep(50);
            return Turn.Step(path[1] - path[0]);
        }
    }
}