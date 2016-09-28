using System;
using System.Collections.Generic;
using System.Linq;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
/*
namespace DummyPlayer
{
    public class BonusCollectionHeuristic : ItemCollectorHeuristic
    {
        public int Distance { get; }
        public bool DefencePriority { get; }
        public bool AttackPriority => !DefencePriority;
        public int PriorityIgnoryEffectiveLevel{ get; }

        public BonusCollectionHeuristic(int distance, bool defencePriority) : base(false)
        {
            Distance = distance;
            DefencePriority = defencePriority;
            PriorityIgnoryEffectiveLevel = 3;
        }

        private int BonusSize(ItemView item) => Math.Max(item.AttackBonus, item.DefenceBonus);

        private int NowBonusSize(Enviroment env)
        {
            var self = env.View.Player;
            var nowDefBonus = self.TotalDefence - self.Defence;
            var nowAttBonus = self.TotalAttack - self.Attack;
            return Math.Max(nowAttBonus, nowDefBonus);
        }

        protected override bool IsCanWork(Enviroment enviroment)
            =>
            enviroment.View.Items
                .Where(i => i.Location.Distance(enviroment.View.Player.Location) <= Distance)
                .Any(i => NowBonusSize(enviroment) < BonusSize(i));

        //TODO: брать самый лучший
        protected override IEnumerable<Location> ExtractItems(Enviroment enviroment)
            =>
            enviroment.View.Items
                .OrderByDescending(BonusSize)
                //.ThenByDescending(i => i.Location.Distance(enviroment.View.Player.Location))
                .Where(i => NowBonusSize(enviroment) < BonusSize(i))
                .Select(i => i.Location);
    }
}*/