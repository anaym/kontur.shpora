using System;
using System.Collections.Generic;
using System.Linq;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Maps
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
                    travable[x, y] = true;
                }
            }
            foreach (var o in objects)
            {
                travable[o.X, o.Y] = false;
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        var loc = o + new Offset(x, y);
                        var d = loc.Distance(o);
                        if (loc.X >= 0 && loc.X < Width && loc.Y >= 0 && loc.Y < Height && d < radius)
                        {
                            var newW = radius - d;
                            weigthes[loc.X, loc.Y] = Math.Max(weigthes[loc.X, loc.Y], newW);
                        }
                    }
                }
            }
        }
    }
}