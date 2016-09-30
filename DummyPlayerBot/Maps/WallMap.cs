using System;
using System.Collections.Generic;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Maps
{
    public class WallMap : BadObjectMap
    {
        public WallMap(LevelView level, int radius = 1) : base(level, (view, l) => view.Field[l] == CellType.Wall, view => view.Field.GetCellsOfType(CellType.Wall), radius)
        {
        }
    }
}