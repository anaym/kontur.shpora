using System.Collections.Generic;
using System.Linq;
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
            Heuristics.Add(new HealingHeuristic(40)); //20
            Heuristics.Add(new TrapLeaverHeuristic());
            Heuristics.Add(new HealingHeuristic(50));
            Heuristics.Add(new BonusCollectorHeuristic());
            Heuristics.Add(new NearAttackHeurisitc());
            Heuristics.Add(new FarAttackHeuristic());
            Heuristics.Add(new HealingHeuristic(99));
            Heuristics.Add(new LevelLeaverHeuristic());
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack)
        {
            isAttack = false;
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap);

            foreach (var heuristic in Heuristics)
            {
                var solve = heuristic.Solve(level, Enviroment, out isAttack);
                if (solve != null)
                    return solve;
            }
            return  Turn.None;
        }

        public Turn HandleCycle(LevelView level) => null;
    }
}