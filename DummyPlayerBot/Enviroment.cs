using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class Enviroment
    {
        public WallMap WallMap;
        public TrapMap TrapMap;
        public EnemyMap EnemyMap;

        public Enviroment(WallMap wallMap, TrapMap trapMap, EnemyMap enemyMap)
        {
            WallMap = wallMap;
            TrapMap = trapMap;
            EnemyMap = enemyMap;
        }

        public void Update(LevelView level, int enemyRadius = 4, int trapRadius = 1)
        {
            TrapMap = new TrapMap(level, trapRadius);
            EnemyMap = new EnemyMap(level, enemyRadius);
        }

        public static Enviroment FromLevelView(LevelView level, int enemyRadius = 4, int trapRadius = 1,  WallMap wallMap = null, int wallRadius = 4)
        {
            if (wallMap == null)
            {
                wallMap = new WallMap(level, wallRadius);
            }
            return new Enviroment(wallMap, new TrapMap(level, trapRadius), new EnemyMap(level, enemyRadius));
        }
    }
}