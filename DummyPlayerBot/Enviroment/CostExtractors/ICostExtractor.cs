using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public interface ICostExtractor
    {
        UDouble GetCost(Location loc);
    }
}