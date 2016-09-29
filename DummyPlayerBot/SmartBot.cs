using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DummyPlayer;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using DummyPlayerBot.Tasks.TrapEscaper;

namespace DummyPlayerBot
{
    public class SmartBot : IBot
    {
        public Location Exit { get; }
        public Location Input { get; }
        public readonly int Index;

        public Enviroment Enviroment;
        public ITask ActualTask;
        public List<ITaskGenerator> EmergencyGenerators;
        public List<ITaskGenerator> Generators;

        public SmartBot(LevelView level, int levelIndex)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            Input = level.Field.GetCellsOfType(CellType.PlayerStart).First();
            Enviroment = Enviroment.FromLevelView(level, 2);
            Index = levelIndex;

            EmergencyGenerators = new List<ITaskGenerator>();
            EmergencyGenerators.Add(new HealingTaskGenerator(50, 100)); //если мало хп - бежим регениться
            //EmergencyGenerators.Add(new TrapEscaperGenerator(hpk: 10)); //
            EmergencyGenerators.Add(new TravelTaskGenerator(l => l.Items.First(i => i.IsBetter(l.Player)).Location, "BONUS", l => l.Items.Any(i => i.IsBetter(l.Player)))); //выбираем дальнюю цель (мега бонус)
            EmergencyGenerators.Add(new NearAttackGenerator());

            Generators = new List<ITaskGenerator>();
            Generators.Add(new FarAttackGenerator()); //выбираем дальнюю цель (врага)
            Generators.Add(new HealingTaskGenerator(99, 99)); //если есть повреждения - хилимся
            Generators.Add(new TravelTaskGenerator(l => Exit, "EXIT")); //сваливаем
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter)
        {
            Enviroment.Update(level, 3);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap);
            if (level.Player.Health < 50)
            {
                var path = travelMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                return Turn.Step(path[1] - path[0]);
            }
            if (level.Items.Any(i => i.IsBetter(level.Player)))
            {
                var path = travelMap.FindPath(level.Player.Location, level.Items.First(i => i.IsBetter(level.Player)).Location);
                return Turn.Step(path[1] - path[0]);
            }
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            if (level.Monsters.Count(m => m.Location.IsInRange(level.Player.Location, 1)) > 1)
            {
                reporter.ReportMessage("ESCAPE");
                var spot = new[] { Exit, Input }.OrderByDescending(s => s.Distance(level.Player.Location)).First();
                var path = travelMap.FindPath(level.Player.Location, spot + new Offset(1, 0));
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
                return Turn.Step(path[1] - path[0]);
            }
            if (level.Player.Health < 100 && level.HealthPacks.Any())
            {
                var path = travelMap.FindPath(level.Player.Location, level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                return Turn.Step(path[1] - path[0]);
            }
            {
                var path = travelMap.FindPath(level.Player.Location, Exit);
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