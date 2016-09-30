using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public interface IAi
    {
        int CriticalPercentageInactivity { get; }
        Location Exit { get; }
        Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack);
        Turn HandleCycle(LevelView level);
    }
}