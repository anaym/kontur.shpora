using System;
using System.Linq;
using DLibrary.Graph;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public class DistanceFromEnemiesCostExtractor : EnviromentDependentCostExtractor
    {
        public DistanceFromEnemiesCostExtractor(Enviroment enviroment) : base(enviroment)
        { }

        public override UDouble GetCost(Location loc)
        {
            //TODO: smart path? Учитывать маршрут врага
            var maxCost = Enviroment.Map.Height + Enviroment.Map.Width;
            if (!Enviroment.View.Monsters.Any())
                return UDouble.Zero;
            return new UDouble(maxCost - Enviroment.View.Monsters.Min(m => m.Location.Distance(loc)));

        }
    }
}