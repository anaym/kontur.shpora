using System;
using System.Reflection;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class TravelTask : ITask
    {
        public TravelTask(Location target, String name)
        {
            Target = target;
            Name = name;
        }

        public Location Target { get; }

        public Turn Step(LevelView level, Enviroment enviroment)
        {
            var map = Map.Sum(enviroment.TrapMap, enviroment.WallMap, enviroment.EnemyMap);
            var path = map.FindPath(level.Player.Location, Target);
            return Turn.Step(path[1] - path[0]);

        }

        public string Name { get; }


        public bool IsFinished(LevelView level, Enviroment enviroment)
        {
            var map = Map.Sum(enviroment.TrapMap, enviroment.WallMap, enviroment.EnemyMap);
            return !map.IsTravaible(Target) || level.Player.Location.Equals(Target);
        }
    }
}