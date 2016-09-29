using System.Linq;
using DLibrary.Graph;
using DummyPlayer;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment.CostExtractors
{
    public class CellTypeDependendCostExtractor : EnviromentDependentCostExtractor
    {
        public double HealingCoefficient = 1;
        public double WallCoefficient = 2;

        public int MaxRadius { get; private set; }
        public UDouble BaseCost { get; set; }

        public CellTypeDependendCostExtractor(Enviroment enviroment, int maxRadius = 1) : base(enviroment)
        {
            MaxRadius = maxRadius;
            BaseCost = (UDouble) 5;
        }

        public UDouble GetEnemyCost(Location loc)
        {
            var enemyDistance = loc
                .RowRectangleAcross(MaxRadius)
                .SelectMany(e => e)
                .Where(i => i.InRect(new Location(0, 0), new Location(Enviroment.Map.Width - 1, Enviroment.Map.Height - 1)))
                .Select(i => Enviroment.View.GetMonsterAt(i))
                .Where(m => m.HasValue)
                .MinOr(i => i.Location.Distance(loc), MaxRadius);

            if (enemyDistance >= MaxRadius) return BaseCost;
            var x = MaxRadius - enemyDistance;
            return (UDouble)(x * x + BaseCost);
        }

        public UDouble GetWallCost(Location loc)
        {
            var wallDistance = loc
                .RowRectangleAcross(MaxRadius)
                .SelectMany(e => e)
                .Where(i => i.InRect(new Location(0, 0), new Location(Enviroment.Map.Width - 1, Enviroment.Map.Height - 1)))
                .Where(i => Enviroment.View.Field[i] == CellType.Wall)
                .MinOr(i => i.Distance(loc), MaxRadius);

            if (wallDistance >= MaxRadius) return BaseCost;
            var x = 2 * MaxRadius - wallDistance;
            return (UDouble)(x * x + BaseCost);
        }

        public override UDouble GetCost(Location loc)
        {
            return GetEnemyCost(loc) + GetWallCost(loc);
        }
    }
}