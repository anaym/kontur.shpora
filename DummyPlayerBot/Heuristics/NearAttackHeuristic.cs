using System.Linq;
using DummyPlayerBot.Enviroment;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public class NearAttackHeuristic : IHeuristic
    {
        public NearAttackHeuristic()
        {
            Status = "[x]";
        }

        public Turn Solve(Enviroment level)
        {
            var self = level.View.Player;
            if (!level.View.Monsters.Any(m => InAttackRange(self.Location, m) && CanKill(self, m, level)))
                return null;
            var enemy = level.View.Monsters.First(m => InAttackRange(self.Location, m) && CanKill(self, m, level));
            Status = $"[{enemy.Name}, {enemy.Health}]";
            return Turn.Attack(enemy.Location - self.Location);
        }

        public string Name => "Near attack";
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