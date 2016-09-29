using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public class TravelCostExtractor : ICostExtractor
    {
        public UDouble GetCost(Location loc) => UDouble.One;
    }
}