using System.Collections.Generic;
using DummyPlayerBot.Enviroment;
using SpurRoguelike.Core.Entities;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public interface IHeuristic
    {
        Turn Solve(Enviroment level);
        string Name { get; }
        string Status { get; }
    }
}