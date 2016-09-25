using System;
using System.Collections.Generic;
using System.Linq;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public static class PawnExtension
    {
        public static double BaseDamage => 10;

        public static double PawnDPS(this PawnView attacker, PawnView defender)
        {
            var dps = (attacker.TotalAttack / defender.TotalDefence) * BaseDamage;
            return dps < 0 ? 0 : dps;
        }

        public static int PawnHealphInSeconds(this PawnView defender, PawnView attacker)
        {
            var aDPS = attacker.PawnDPS(defender);
            return (int) Math.Floor(defender.Health/(aDPS == 0 ? 1 : aDPS));
        }

        public static int PawnHealphInSeconds(this PawnView defender, IList<PawnView> attacker, double radius = 5)
        {
            var aDPS = attacker.Select(m => m.PawnDPS(defender)).Sum();
            return (int)Math.Floor(defender.Health / (aDPS == 0 ? 1 : aDPS)) + attacker.Min(a => a.Location.Distance(defender.Location));
        }
    }
}