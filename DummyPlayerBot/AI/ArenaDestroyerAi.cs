﻿using System.Diagnostics;
using System.Linq;
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
        public long RemaingDistance { get; set; }
        public int CriticalPercentageInactivity => 40;

        public ArenaDestroyerAi(LevelView level)
        {
            Enviroment = new Enviroment(level, 2);
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            CriticalTime = 100;
            Time = new Stopwatch();
            Time.Start();
        }

        public Enviroment Enviroment { get; set; }

        public Location Exit { get; }
        public Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack)
        {
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap, bonusIgnore);
            if (level.Player.Health < 50 && level.HealthPacks.Any())
            {
                var path = travelMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                isAttack = false;
                return Turn.Step(path[1] - path[0]);
            }
            //if (level.Items.Any(i => i.IsBetter(level.Player)))
            //{
            //    var path = travelMap.FindPath(level.Player.Location, level.Items.First(i => i.IsBetter(level.Player)).Location);
            //    return Turn.Step(path[1] - path[0]);
            //}
            if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                var monster = level.Monsters.Where(m => m.Location.IsInRange(level.Player.Location, 1)).OrderBy(m => m.Health).First();
                isAttack = true;
                return Turn.Attack(monster.Location - level.Player.Location);
            }
            if (level.Monsters.Any() /*&& level.Monsters.First().Location.Distance(level.Player.Location) > 1*/)
            {
                var path = attackMap.FindPath(level.Player.Location, level.Monsters.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                isAttack = false;
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            if (!ExitIsClosed(level))
            {
                Enviroment = new Enviroment(level, 2);
                var path = travelMap.FindPath(level.Player.Location, Exit);
                isAttack = false;
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            isAttack = false;
            return Turn.None;
        }

        public bool ExitIsClosed(LevelView level)
        {
            var exit = level.Field.GetCellsOfType(CellType.Exit).First();
            var neir = new Location[] { exit.Up(), exit.Down(), exit.Left(), exit.Right() };
            return neir.All(p => level.Field[p] == CellType.Wall);
        }
    }
}