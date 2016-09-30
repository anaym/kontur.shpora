using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public interface IAiFactory
    {
        IAi CreateBot(LevelView level, int levelNumber);
    }
}