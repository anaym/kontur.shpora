using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DummyPlayerBot.AI.Heuristics;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public class SmartAi : IAi
    {
        public int CriticalPercentageInactivity => 40;
        public Location Exit { get; }
        public Location Input { get; }
        public readonly int Index;

        public List<IHeuristic> Heuristics;

        public Enviroment Enviroment;

        public SmartAi(LevelView level, int levelIndex)
        {
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            Input = level.Field.GetCellsOfType(CellType.PlayerStart).First();
            Enviroment = new Enviroment(level, 2);
            Index = levelIndex;

            Heuristics = new List<IHeuristic>();
            Heuristics.Add(new NearHealingHeuristic(50)); //20
            Heuristics.Add(new HealingHeuristic(30)); //20
            Heuristics.Add(new HealingHeuristic(40, false)); //20
            Heuristics.Add(new TrapLeaverHeuristic());
            Heuristics.Add(new HealingHeuristic(50));
            Heuristics.Add(new NearAttackHeurisitc());
            Heuristics.Add(new FarAttackHeuristic());
            Heuristics.Add(new BonusCollectorHeuristic());
            Heuristics.Add(new HealingHeuristic(99));
            Heuristics.Add(new LevelLeaverHeuristic());
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack)
        {
            if (Index == 21)
                Thread.Sleep(150);

            isAttack = false;
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap);

            foreach (var heuristic in Heuristics)
            {
                var solve = heuristic.Solve(level, Enviroment, out isAttack);
                if (solve != null)
                {
                    reporter.ReportMessage(heuristic.GetType().Name + " " + level.Player.Health);
                    return solve;
                }
            }
            return  Turn.None;
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