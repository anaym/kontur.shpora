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
        public readonly int Index;

        public Enviroment Enviroment;
        public ITask ActualTask;
        public List<ITaskGenerator> EmergencyGenerators;
        public List<ITaskGenerator> Generators;

        public SmartBot(LevelView level, int levelIndex)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            Enviroment = Enviroment.FromLevelView(level);
            Index = levelIndex;

            EmergencyGenerators = new List<ITaskGenerator>();
            EmergencyGenerators.Add(new HealingTaskGenerator(60)); //если мало хп - бежим регениться
            EmergencyGenerators.Add(new TravelTaskGenerator(l => l.Items.First(i => i.IsBetter(l.Player)).Location, "BONUS", l => l.Items.Any(i => i.IsBetter(l.Player)))); //выбираем дальнюю цель (мега бонус)
            EmergencyGenerators.Add(new NearAttackGenerator());

            Generators = new List<ITaskGenerator>();
            Generators.Add(new FarAttackGenerator()); //выбираем дальнюю цель (врага)
            Generators.Add(new HealingTaskGenerator(99)); //если есть повреждения - хилимся
            Generators.Add(new TravelTaskGenerator(l => Exit, "EXIT")); //сваливаем
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter)
        {
            Enviroment.Update(level);

            foreach (var emergencyGenerator in EmergencyGenerators)
            {
                if (emergencyGenerator.CanReplace(ActualTask, level, Enviroment))
                {
                    ActualTask = emergencyGenerator.Generate(level, Enviroment);
                    break;
                }
            }

            if (ActualTask == null || ActualTask.IsFinished(level, Enviroment))
            {
                foreach (var taskGenerator in Generators)
                {
                    if (taskGenerator.CanReplace(ActualTask, level, Enviroment))
                    {
                        ActualTask = taskGenerator.Generate(level, Enviroment);
                        break;
                    }
                }
            }

            if (ActualTask != null && !ActualTask.IsFinished(level, Enviroment))
            {
                reporter.ReportMessage(ActualTask.Name);
                Thread.Sleep(10);
                return ActualTask.Step(level, Enviroment);
            }
            return Turn.None;
            

            //если мало хп - бежим регениться
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            //если можем атаковать - атакуем [и если есть аптечки на уровне]
            //бежим к дальней цели
            //выбираем дальнюю цель (врага, мега бонус)
            //если есть повреждения - хилимся
            //сваливаем
            throw new System.NotImplementedException();
        }
    }
}