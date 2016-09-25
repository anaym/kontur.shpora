using System;
using System.Collections.Generic;
using System.Linq;
using DLibrary.Graph;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayer
{
    public static class LocationExtension
    {
        public static Location Up(this Location location) => location.WithDelta(0, -1);
        public static Location Down(this Location location) => location.WithDelta(0, 1);
        public static Location Left(this Location location) => location.WithDelta(-1, 0);
        public static Location Right(this Location location) => location.WithDelta(1, 0);
        public static Location WithDelta(this Location location, int dx, int dy) => new Location(location.X + dx, location.Y + dy);
        public static int Distance(this Location a, Location b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        public static bool InRect(this Location a, Location leftTop, Location rightBottom) => a.X.InRange(leftTop.X, rightBottom.X) && a.Y.InRange(leftTop.Y, rightBottom.Y);

        public static IEnumerable<Turn> ToTurn(this IEnumerable<Location> locations) => locations.NGramm(2).Select(p => Turn.Step(p[1] - p[0]));

        public static IEnumerable<IEnumerable<Location>> RowRectangleAcross(this Location leftTop, Location rightBottom)
        {
            for (int x = leftTop.X; x <= rightBottom.X; x++)
            {
                yield return Enumerable.Range(leftTop.Y, rightBottom.Y - leftTop.Y + 1).Select(y => new Location(x, y));
            }
        }

        public static IEnumerable<IEnumerable<Location>> RowRectangleAcross(this Location centre, int widthRadius, int heightRadius)
        {
            return
                new Location(centre.X - widthRadius, centre.Y - heightRadius)
                    .RowRectangleAcross(new Location(centre.X + widthRadius, centre.Y + heightRadius));
        }

        public static IEnumerable<IEnumerable<Location>> RowRectangleAcross(this Location centre, int radius)
        {
            return centre.RowRectangleAcross(radius, radius);
        }
    }

    public static class DoubleExtension
    {
        public static bool InRange(this double a, double left, double right) => a >= Math.Min(left, right) && a <= Math.Max(left, right);
        public static bool InRange(this int a, double left, double right) => ((double) a).InRange(left, right);
    }
}