using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public interface IBot
    {
        Location Exit { get; }
        Turn Iteration(LevelView level, IMessageReporter reporter);
    }
}