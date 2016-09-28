using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public interface IBotFactory
    {
        IBot CreateBot(LevelView level, int levelNumber);
    }
}