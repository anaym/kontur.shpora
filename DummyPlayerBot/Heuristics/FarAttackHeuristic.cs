using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public class FarAttackHeuristic : IHeuristic
    {
        public int Distance { get; }

        public FarAttackHeuristic(int distance)
        {
            Distance = distance;
            Status = "[x]";
        }

        public Turn Solve(Enviroment level)
        {
            var self = level.View.Player;
            if (!level.View.Monsters.Where(m => m.Location.Distance(self.Location) < Distance).Any(m => CanKill(self, m, level)))
                return null;
            var enemy = level.View.Monsters.Where(m => m.Location.Distance(self.Location) < Distance).OrderBy(m => m.Health).First(m => CanKill(self, m, level));
            Status = $"[{enemy.Name}: {enemy.Location.Distance(self.Location)}]";
            return
                level.Map.FindPath(self.Location, enemy.Location, new PositiveWeighesPathFinder(),
                    new TravelCostExtractor(), ignoreEnemy: false).ToTurn().First();
        }

        public string Name => "Far attack";
        public string Status { get; private set; }

        //TODO: учитывать подкрепление
        private static bool CanKill(PawnView player, PawnView pawn, Enviroment level)
        {
            var ePHIS = pawn.PawnHealphInSeconds(player);
            var pPHIS = player.PawnHealphInSeconds(level.View.Monsters.ToList());
            return ePHIS < pPHIS;
        }

        private static bool InAttackRange(Location pos, PawnView pawn) => pawn.Location.IsInRange(pos, 1);
    }
}