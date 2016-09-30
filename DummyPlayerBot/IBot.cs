using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public interface IBot
    {
        int CriticalPercentageInactivity { get; }
        Location Exit { get; }
        Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack);
    }
}