using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DummyPlayer;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class SmartBot : IBot
    {
        public Location Exit { get; }
        public Location Input { get; }
        public readonly int Index;

        public Enviroment Enviroment;
        public ITask ActualTask;

        public SmartBot(LevelView level, int levelIndex)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            Input = level.Field.GetCellsOfType(CellType.PlayerStart).First();
            Enviroment = Enviroment.FromLevelView(level, 2);
            Index = levelIndex;
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter)
        {
            if (Index == 100500)
                Thread.Sleep(100);
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap);
            var bonusCollectorMap = Map.Sum(Enviroment.WallMap, Enviroment.EnemyMap, Enviroment.TrapMap);
            if (level.Player.Health < 50 && level.HealthPacks.Any())
            {
                Enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(travelMap, Enviroment.EnemyMap);
                Enviroment.EnemyMap.Multiplyer = 2;
                
                foreach (var hp in level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)))
                {
                    var path = map.FindPath(level.Player.Location, hp.Location);
                    if (path == null)
                    {
                        if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
                        {
                            return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
                        }

                    }
                    else
                    {
                        return Turn.Step(path[1] - path[0]);
                    }
                }
                return Turn.None;
            }
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            if (level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1)
            {
                Enviroment.EnemyMap.Multiplyer = 2;
                var map = Map.Sum(travelMap, Enviroment.EnemyMap);
                Enviroment.EnemyMap.Multiplyer = 2;
                reporter.ReportMessage("ESCAPE");
                var spot = new[] { Exit, Input }.OrderByDescending(s => s.Distance(level.Player.Location)).First();
                var path = map.FindPath(level.Player.Location, spot + new Offset(1, 0));
                if (path == null)
                    if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
                        return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
                    else
                        return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            if (level.Items.Any(i => i.IsBetter(level.Player)))
            {
                var path = bonusCollectorMap.FindPath(level.Player.Location, level.Items.First(i => i.IsBetter(level.Player)).Location);
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                var monster = level.Monsters.Where(m => m.Location.IsInRange(level.Player.Location, 1)).OrderBy(m => m.Health).First();
                return Turn.Attack(monster.Location - level.Player.Location);
            }
            if (level.Monsters.Any())
            {
                var path = attackMap.FindPath(level.Player.Location, level.Monsters.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                if (path == null)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            if (level.Player.Health < 100 && level.HealthPacks.Any())
            {
                foreach (var hp in level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)))
                {
                    var path = travelMap.FindPath(level.Player.Location, hp.Location);
                    if (path == null)
                    {
                        if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
                        {
                            return Turn.Attack(level.Monsters.First(m => m.Location.IsInRange(level.Player.Location, 1)).Location - level.Player.Location);
                        }

                    }
                    else
                    {
                        return Turn.Step(path[1] - path[0]);
                    }
                }
                return Turn.None;
            }
            {
                var path = travelMap.FindPath(level.Player.Location, Exit);
                if (path == null)
                {
                    path = bonusCollectorMap.FindPath(level.Player.Location, Exit);
                }
                if (path == null)
                {
                    return Turn.None;
                }
                return Turn.Step(path[1] - path[0]);
            }
            



            //если мало хп - бежим регениться
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            //если можем атаковать - атакуем [и если есть аптечки на уровне]
            //бежим к дальней цели
            //выбираем дальнюю цель (врага, мега бонус)
            //если есть повреждения - хилимся
            //сваливаем

            //throw new System.NotImplementedException();
            return Turn.None;
        }
    }
}