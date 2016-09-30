using System.Linq;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Maps
{
    public class EnemyMap : BadObjectMap
    {
        public EnemyMap(LevelView level, int radius = 3) : base(level, (view, l) => view.Monsters.Any(m => m.Location.Equals(l)), view => view.Monsters.Select(m => m.Location), radius)
        {
        }
    }
}