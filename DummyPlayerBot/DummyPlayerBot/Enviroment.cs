using System.Linq;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class Enviroment
    {
        public WallMap WallMap;
        public TrapMap TrapMap;
        public EnemyMap EnemyMap;
        public Map TravelMap;
        public Map AttackMap;
        public Map BonusIgnore;

        public Location Exit { get; }
        public Location Start { get; }

        public Enviroment(LevelView level, int enemyRadius = 4, int trapRadius = 1, WallMap wallMap = null, int wallRadius = 4)
        {
            if (wallMap == null)
            {
                wallMap = new WallMap(level, wallRadius);
            }
            WallMap = wallMap;
            TrapMap = new TrapMap(level, trapRadius);
            EnemyMap = new EnemyMap(level, enemyRadius);

            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            Start = level.Field.GetCellsOfType(CellType.PlayerStart).First();
        }

        public void Update(LevelView level, int enemyRadius = 4, int trapRadius = 1)
        {
            TrapMap = new TrapMap(level, trapRadius);
            EnemyMap = new EnemyMap(level, enemyRadius);
            FillAdditionMaps(level);
        }

        private void FillAdditionMaps(LevelView level)
        {
            BonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            AttackMap = Map.Sum(WallMap, TrapMap, BonusIgnore);
            TravelMap = Map.Sum(AttackMap, EnemyMap);
        }
    }
}