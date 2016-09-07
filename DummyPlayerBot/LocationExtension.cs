using System;
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
    }
}