using System;
using System.Collections.Generic;
using System.Linq;
using DummyPlayerBot.Enviroment;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayer
{
    public class HealingHeuristic : ItemCollectorHeuristic
    {
        public int NormalHpLevel { get; }

        public HealingHeuristic(int normalHpLevel)
        {
            NormalHpLevel = normalHpLevel;
        }

        protected override bool IsCanWork(Enviroment enviroment) => enviroment.View.Player.Health < NormalHpLevel;

        protected override IEnumerable<Location> ExtractItems(Enviroment enviroment) => enviroment.View.HealthPacks.Select(p => p.Location);
    }
}