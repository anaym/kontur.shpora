using System.Collections.Generic;
using System.Linq;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public abstract class ItemCollectorHeuristic : IHeuristic
    {
        protected PositiveWeighesPathFinder PathFinder;
        protected abstract bool IsCanWork(Enviroment enviroment);
        protected abstract IEnumerable<Location> ExtractItems(Enviroment enviroment);
        private readonly bool collectNearest;

        protected ItemCollectorHeuristic(bool collectNearest = true)
        {
            PathFinder = new PositiveWeighesPathFinder();
            this.collectNearest = collectNearest;
            Status = "[x]";
        }

        public Turn Solve(Enviroment level)
        {
            if (!IsCanWork(level))
                return null;
            var self = level.View.Player.Location;
            var items = ExtractItems(level);
            if (collectNearest)
                items = items.OrderBy(l => l.Distance(self));
            var nearest = items.FirstOr(new Location(int.MinValue, int.MinValue));
            if (nearest.X == int.MinValue)
                return null;
            Status = $"[{nearest.GetType()}]";
            var path = level.Map.FindPath(self, nearest, PathFinder, new DistanceFromEnemiesCostExtractor(level));
            return path.ToTurn().FirstOr(null);
        }

        public string Name => GetType().ToString();
        public string Status { get; private set; }
    }
}