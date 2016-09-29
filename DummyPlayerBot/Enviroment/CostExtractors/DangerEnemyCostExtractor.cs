using System.Linq;
using DLibrary.Graph;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public class DangerEnemyCostExtractor : EnviromentDependentCostExtractor
    {
        public DangerEnemyCostExtractor(Enviroment enviroment) : base(enviroment)
        { }

        public override UDouble GetCost(Location loc)
        {
            //TODO: smart path? Учитывать маршрут врага
            var maxCost = (Enviroment.Map.Height + Enviroment.Map.Width)*1000000;
            if (!Enviroment.View.Monsters.Any())
                return UDouble.Zero;
            return new UDouble(maxCost - Enviroment.View.Player.PawnHealphInSeconds(Enviroment.View.Monsters.ToList()));

        }
    }
}