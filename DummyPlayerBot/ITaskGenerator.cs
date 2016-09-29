using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public interface ITaskGenerator
    {
        bool CanReplace(ITask task, LevelView level, Enviroment enviroment);
        ITask Generate(LevelView level, Enviroment enviroment);
    }
}