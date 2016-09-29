using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpurRoguelike.Core.Views;
using DummyPlayerBot;
using DummyPlayer;

namespace DummyPlayerBot.Tasks.TrapEscaper
{
    public class TrapEscaperGenerator : ITaskGenerator
    {
        public readonly double HpK;
        public readonly int Radius;
        public TrapEscaperGenerator(double hpk = 2, int radius = 20)
        {
            HpK = hpk;
            Radius = radius;
        }

        public bool CanReplace(ITask task, LevelView level, Enviroment enviroment)
        {/*
            if (!(task is TrapEscapeTask))
                return false;*/
            return IsTrap(level);
        }

        public ITask Generate(LevelView level, Enviroment enviroment)
        {
            var hpb = level.HealthPacks.OrderByDescending(h => h.Location.Distance(level.Player.Location)).First();
            return new TravelTask(hpb.Location, "ESCAPE");
        }

        public bool IsTrap(LevelView level)
        {
            return level.Monsters
                .Where(m => m.Location.Distance(level.Player.Location) < Radius)
                .Sum(m => m.Health) > level.Player.Health * HpK;
        }
    }
}
