using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public interface ITask
    {
        String Name { get; }
        Turn Step(LevelView level, Enviroment enviroment);
        bool IsFinished(LevelView level, Enviroment enviroment);
    }
}
