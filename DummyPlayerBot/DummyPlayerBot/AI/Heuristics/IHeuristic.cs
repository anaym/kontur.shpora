using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI.Heuristics
{
    public interface IHeuristic
    {
        Turn Solve(LevelView level, Enviroment enviroment, out bool isAttack);
    }
}