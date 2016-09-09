using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public class CellTypeDependendCostExtractor : EnviromentDependentCostExtractor
    {
        public double HealingCoefficient = 1;
        public double WallCoefficient = 2;

        public CellTypeDependendCostExtractor(Enviroment enviroment) : base(enviroment)
        { }

        public override UDouble GetCost(Location loc)
        {
            throw new System.NotImplementedException();
        }
    }
}