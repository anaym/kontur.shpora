using System;
using System.Collections.Generic;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Maps
{
    public class TrapMap : BadObjectMap
    {
        public TrapMap(LevelView level, int radius = 1) : base(level, (view, l) => view.Field[l] == CellType.Trap, view => view.Field.GetCellsOfType(CellType.Trap), radius)
        {
        }
    }
}