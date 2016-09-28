using System.Linq;
using DummyPlayerBot.FastKillBot;

namespace DummyPlayer
{
    public class EnemyCountDependendHealingHeuristic : HealingHeuristic
    {
        public int MaxHpLevel { get; }

        public EnemyCountDependendHealingHeuristic(int normalHpLevel, int maxHpLevel) : base(normalHpLevel)
        {
            MaxHpLevel = maxHpLevel;
        }

        public int ActualHpLevel(Enviroment enviroment)
        {
            var self = enviroment.View.Player;
            var nearEnemy = enviroment.View.Monsters.Count(e => e.Location.Distance(self.Location) < 10);
            var totalEnemy = enviroment.View.Monsters.Count();
            if (nearEnemy == 1)
                return NormalHpLevel;
            return NormalHpLevel + (int)((MaxHpLevel - NormalHpLevel)*(1.0*nearEnemy/totalEnemy));
        }

        protected override bool IsCanWork(Enviroment enviroment)
        {
            return enviroment.View.Player.Health < ActualHpLevel(enviroment);
        }
    }
}