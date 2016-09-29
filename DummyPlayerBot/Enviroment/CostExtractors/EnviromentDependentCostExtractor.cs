using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public abstract class EnviromentDependentCostExtractor : ICostExtractor
    {
        public Enviroment Enviroment { get; }

        protected EnviromentDependentCostExtractor(Enviroment enviroment)
        {
            Enviroment = enviroment;
        }

        public abstract UDouble GetCost(Location loc);
    }
}