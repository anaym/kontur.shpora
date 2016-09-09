using System.Linq;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayer
{
    public class SuicideAttack : IHeuristic
    {
        public SuicideAttack()
        { }

        public Turn Solve(Enviroment level)
        {
            var self = level.View.Player;
            if (!level.View.Monsters.Any())
                return null;
            var enemy = level.View.Monsters.OrderBy(m => m.Health).First();
            return
                level.Map.FindPath(self.Location, enemy.Location, new PositiveWeighesPathFinder(),
                    new TravelCostExtractor(), ignoreEnemy: false).ToTurn().First();
        }

        public string Name => "Suicide attack";
        public string Status => "[v]";
    }
}