using System;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public static class BonusExtensions
    {
        public static bool IsBetter(this ItemView item, PawnView me)
        {
            var ab = me.TotalAttack - me.Attack;
            var db = me.TotalDefence - me.Defence;
            var tb = Math.Max(ab, db);
            var itb = Math.Max(item.AttackBonus, item.DefenceBonus);
            return tb < itb || tb == itb && (ab + db) < (item.AttackBonus + item.DefenceBonus);
        }
    }
}