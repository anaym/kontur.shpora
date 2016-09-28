using System;
using System.Collections.Generic;
using System.Linq;
using DummyPlayer;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class BadObjectMap : Map
    {
        public BadObjectMap(LevelView level, Func<LevelView, Location, bool> isObject, Func<LevelView, IEnumerable<Location>> getObjects, int radius = 4) : base(level.Field.Width, level.Field.Height)
        {
            var objects = getObjects(level).ToList();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var now = new Location(x, y);
                    SetTravaible(x, y, !isObject(level, now));
                    SetWeight(x, y, objects.Where(w => w.Distance(now) < radius).MaxOr(w => radius - w.Distance(now)));
                }
            }
        }
    }
}