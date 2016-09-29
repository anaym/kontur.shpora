using System;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Entities;
using SpurRoguelike.Core.Primitives;

namespace SpurRoguelike
{
    public class NullIO : IRenderer, IEventReporter
    {
        public int LevelsCompleted = 0;
        public int GameComleted = 0;
        public int NowLevel = 0;
        public int NowGame = 0;

        public NullIO(int firstSeed)
        {
            NowGame = firstSeed;
            Console.WriteLine($"Game has started: {NowGame}");
        }

        public void RenderLevel(Level level)
        { }

        public void RenderGameEnd(bool isCompleted)
        {
            GameComleted += isCompleted ? 1 : 0;
            Console.WriteLine($"Game {(isCompleted ? "completed" : "fail")}!");
            NowGame++;
            NowLevel = 0;
            Console.WriteLine($"Game has started: {NowGame}");
        }

        public void ReportMessage(string message)
        { }

        public void ReportMessage(Entity instigator, string message)
        { }

        public void ReportLevelEnd()
        {
            LevelsCompleted++;
            Console.WriteLine($"Level {NowLevel} done!");
            NowLevel++;
        }

        public void ReportGameEnd()
        { }

        public void ReportNewEntity(Location location, Entity entity)
        { }

        public void ReportAttack(Pawn attacker, Pawn victim)
        { }

        public void ReportDamage(Pawn pawn, int damage, Entity instigator)
        { }

        public void ReportTrap(Pawn pawn)
        { }

        public void ReportPickup(Pawn pawn, Pickup item)
        { }

        public void ReportDestroyed(Pawn pawn)
        { }

        public void ReportUpgrade(Pawn pawn, int attackBonus, int defenceBonus)
        { }
    }
}