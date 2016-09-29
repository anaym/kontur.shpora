using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class HealingTaskGenerator : ITaskGenerator
    {
        public int CriticalHpLevel;
        public int MaxCriticalHpLevel;
        public HealingTaskGenerator(int criticalHpLevel, int maxCriticalHpLevel)
        {
            CriticalHpLevel = criticalHpLevel;
            MaxCriticalHpLevel = maxCriticalHpLevel;
        }


        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {
            var near = level.Monsters.Count(m => m.Location.Distance(level.Player.Location) < 20);
            var all = level.Monsters.Count();
            all = all == 0 ? 1 : all;
            var nowCritical = CriticalHpLevel + (MaxCriticalHpLevel - CriticalHpLevel) * (1.0*near/all);
            if (level.Player.Health < nowCritical)
            {
                if (task is TravelTask && level.HealthPacks.Any(p => p.Location.Equals((task as TravelTask).Target)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public ITask Generate(LevelView level, Enviroment enviroment)
        {
            return new TravelTask(level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location, "HEALING");
        }
    }
}