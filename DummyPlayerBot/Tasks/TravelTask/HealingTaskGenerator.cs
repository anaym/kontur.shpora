using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class HealingTaskGenerator : ITaskGenerator
    {
        public int CriticalHpLevel;
        public HealingTaskGenerator(int criticalHpLevel)
        {
            CriticalHpLevel = criticalHpLevel;
        }


        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {
            if (level.Player.Health < CriticalHpLevel)
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