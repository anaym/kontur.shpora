using System;
using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class FarAttack : ITask
    {
        public readonly String MonsterName;

        public FarAttack(String monsterName)
        {
            MonsterName = monsterName;
        }


        public string Name => "Far attack";

        public Turn Step(LevelView level, Enviroment enviroment)
        {
            var map = Map.Sum(enviroment.TrapMap, enviroment.WallMap);
            var path = map.FindPath(level.Player.Location, level.Monsters.Where(m => !m.IsDestroyed).First(m => m.Name.Equals(MonsterName)).Location);
            return Turn.Step(path[1] - path[0]);

        }

        public bool IsFinished(LevelView level, Enviroment enviroment)
        {
            return !level.Monsters.Where(m => !m.IsDestroyed).Any(m => m.Name.Equals(MonsterName));
        }
    }
}