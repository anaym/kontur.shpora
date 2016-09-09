using System.Linq;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayer
{
    public class LevelLeavingHeuristic : IHeuristic
    {
        public Turn Solve(Enviroment level)
        {
            var path = level.Map.FindPath(level.View.Player.Location, level.Exit, new PositiveWeighesPathFinder(), new DistanceFromEnemiesCostExtractor(level));
            return path.ToTurn().First();
        }

        public string Name => "Level leaving";
        public string Status => "[v]";
    }
}