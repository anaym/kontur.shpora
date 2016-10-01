using System.Linq;
using DummyPlayerBot.Extension;
using DummyPlayerBot.Maps;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.AI
{
    public class ArenaDestroyerAi : IAi
    {
        public int CriticalTime { get; set; }
        public int CriticalPercentageInactivity => 40;
        public int MonsterStartHp { get; }

        public ArenaDestroyerAi(LevelView level)
        {
            Enviroment = new Enviroment(level, 2);
            Exit = level.Field.GetCellsOfType(CellType.Exit).First();
            CriticalTime = 100;

            if (level.Monsters.Any())
                MonsterStartHp = level.Monsters.First().Health;
        }

        public Enviroment Enviroment { get; set; }

        public Location Exit { get; }
        public Turn Iteration(LevelView level, IMessageReporter reporter, out bool isAttack)
        {
            Enviroment.Update(level, 3);
            var bonusIgnore = new BadObjectMap(level, (view, location) => level.Items.Any(i => i.Location.Equals(location)), view => level.Items.Select(i => i.Location), 1);
            var attackMap = Map.Sum(Enviroment.WallMap, Enviroment.TrapMap, bonusIgnore);
            var travelMap = Map.Sum(attackMap, Enviroment.EnemyMap, bonusIgnore);
            if (level.Monsters.Any())
            {
                var monster = level.Monsters.First();
                var enemyHp = monster.Health;
                var healingHpLevel = 50;
                if (enemyHp < MonsterStartHp*0.6) //если враг пытается отрегениться - забираем его аптечку))
                    healingHpLevel = 60;
                if (level.Player.Health < healingHpLevel && level.HealthPacks.Any())
                {
                    var path = travelMap.FindPath(level.Player.Location,
                        level.HealthPacks.OrderBy(h => h.Location.Distance(level.Player.Location)).First().Location);
                    isAttack = false;
                    if (path != null && path.Count > 1)
                        return Turn.Step(path[1] - path[0]);
                    return Turn.None;
                }
            }

            if (level.Monsters.Any(m => m.Location.IsInRange(level.Player.Location, 1)))
            {
                var monster = level.Monsters.Where(m => m.Location.IsInRange(level.Player.Location, 1)).OrderBy(m => m.Health).First();
                isAttack = true;
                return Turn.Attack(monster.Location - level.Player.Location);
            }
            if (level.Monsters.Any())
            {
                var target = level.Monsters.First().Location;
                var targets = target
                    .Near(3)
                    .Where(
                        p =>
                            p.X >= 0 && p.Y >= 0 && p.X < Enviroment.TravelMap.Width &&
                            p.Y < Enviroment.TravelMap.Height)
                    .Where(p => Enviroment.TravelMap.IsTravaible(p))
                    .OrderBy(p => p.Distance(target));

                foreach (var location in targets)
                {
                    var path = attackMap.FindPath(level.Player.Location, location);
                    isAttack = false;
                    if (path != null && path.Count > 1)
                        return Turn.Step(path[1] - path[0]);
                }
            }
            if (!ExitIsClosed(level))
            {
                Enviroment = new Enviroment(level, 2);
                var path = travelMap.FindPath(level.Player.Location, Exit);
                isAttack = false;
                if (path == null || path.Count < 2)
                    return Turn.None;
                return Turn.Step(path[1] - path[0]);
            }
            isAttack = false;
            return Turn.None;
        }

        public Turn HandleCycle(LevelView level) => null;

        public bool ExitIsClosed(LevelView level)
        {
            var exit = level.Field.GetCellsOfType(CellType.Exit).First();
            var neir = new [] { exit.Up(), exit.Down(), exit.Left(), exit.Right() };
            return neir.All(p => level.Field[p] == CellType.Wall);
        }
    }
}
